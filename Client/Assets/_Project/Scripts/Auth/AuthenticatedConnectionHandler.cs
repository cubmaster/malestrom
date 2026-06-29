using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Auth
{
    /// <summary>
    /// Integrates auth token into Netcode for GameObjects connection flow.
    ///
    /// Client-side: Attaches JWT to ConnectionApprovalRequest.Payload
    /// Server-side: Validates token from ConnectionApprovalRequest.Payload
    ///
    /// BR-4: When auth mode is disabled, connection proceeds without token validation.
    /// </summary>
    public class AuthenticatedConnectionHandler : MonoBehaviour
    {
        [SerializeField] AuthConfig _config;
        [SerializeField] string _authServiceUrl = "http://localhost:3000";

        ServerTokenValidator _validator;

        void Awake()
        {
            var authEnabled = _config != null ? _config.AuthModeEnabled : true;
            var url = _config != null ? _config.AuthServiceUrl : _authServiceUrl;
            _validator = new ServerTokenValidator(url, authEnabled);
        }

        void Start()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null) return;

            // On client: attach token to connection payload
            nm.NetworkConfig.ConnectionApproval = true;
            nm.ConnectionApprovalCallback += OnConnectionApproval;
        }

        void OnDestroy()
        {
            var nm = NetworkManager.Singleton;
            if (nm != null)
            {
                nm.ConnectionApprovalCallback -= OnConnectionApproval;
            }
        }

        /// <summary>
        /// Call this on the CLIENT before starting connection to encode the auth token.
        /// </summary>
        public static void SetConnectionPayload(NetworkManager networkManager, string token)
        {
            if (networkManager == null) return;

            if (string.IsNullOrEmpty(token))
            {
                networkManager.NetworkConfig.ConnectionData = new byte[0];
                Debug.Log("[AuthConnection] No token set (auth may be disabled).");
                return;
            }

            networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(token);
            Debug.Log("[AuthConnection] Token attached to connection payload.");
        }

        /// <summary>
        /// Server-side connection approval with token validation.
        /// </summary>
        async void OnConnectionApproval(
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            // Only validate on server
            if (!NetworkManager.Singleton.IsServer)
            {
                response.Approved = true;
                response.CreatePlayerObject = false;
                return;
            }

            var authEnabled = _config != null ? _config.AuthModeEnabled : true;

            if (!authEnabled)
            {
                Debug.Log("[AuthConnection] Auth mode disabled - approving connection (dev bypass).");
                response.Approved = true;
                response.CreatePlayerObject = false;
                return;
            }

            // Extract token from connection payload
            string token = null;
            if (request.Payload != null && request.Payload.Length > 0)
            {
                token = Encoding.UTF8.GetString(request.Payload);
            }

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogWarning("[AuthConnection] Connection rejected: no auth token provided.");
                response.Approved = false;
                response.CreatePlayerObject = false;
                response.Reason = "No authentication token provided.";
                return;
            }

            // Validate token with auth service
            var result = await _validator.ValidateAsync(token);

            if (result.IsValid)
            {
                Debug.Log($"[AuthConnection] Connection approved for account: {result.AccountId}");
                response.Approved = true;
                response.CreatePlayerObject = false;
            }
            else
            {
                Debug.LogWarning($"[AuthConnection] Connection rejected: {result.Reason}");
                response.Approved = false;
                response.CreatePlayerObject = false;
                response.Reason = $"Authentication failed: {result.Reason}";
            }
        }
    }
}
