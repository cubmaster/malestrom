using UnityEngine;

namespace IronExiles.Auth
{
    /// <summary>
    /// Manages the authentication session state.
    /// Stores/retrieves token from PlayerPrefs (local dev).
    /// In production, use secure storage.
    /// </summary>
    public class AuthSessionManager : MonoBehaviour
    {
        [SerializeField] AuthConfig _config;

        AuthClient _client;

        public static AuthSessionManager Instance { get; private set; }

        /// <summary>Current auth token (null if not logged in).</summary>
        public string Token { get; private set; }

        /// <summary>Current account ID (null if not logged in).</summary>
        public string AccountId { get; private set; }

        /// <summary>Whether the user is currently authenticated.</summary>
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        /// <summary>Whether auth mode is enabled (BR-4).</summary>
        public bool AuthModeEnabled => _config != null && _config.AuthModeEnabled;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_config == null)
            {
                Debug.LogError("[AuthSessionManager] AuthConfig not assigned!");
                return;
            }

            _client = new AuthClient(_config.AuthServiceUrl);

            // Try to restore previous session
            Token = PlayerPrefs.GetString(_config.TokenStorageKey, null);
            AccountId = PlayerPrefs.GetString(_config.AccountIdStorageKey, null);

            if (!string.IsNullOrEmpty(Token))
            {
                Debug.Log("[AuthSessionManager] Restored previous session token.");
            }
        }

        /// <summary>
        /// Register a new account.
        /// </summary>
        public async void Register(string email, string password, System.Action<bool, string> callback)
        {
            var result = await _client.RegisterAsync(email, password);

            if (result.Success)
            {
                Debug.Log($"[AuthSessionManager] Registered: {result.Data.account_id}");
                callback?.Invoke(true, null);
            }
            else
            {
                Debug.LogWarning($"[AuthSessionManager] Registration failed: {result.ErrorMessage}");
                callback?.Invoke(false, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Login with email and password. Stores token on success.
        /// </summary>
        public async void Login(string email, string password, System.Action<bool, string> callback)
        {
            var result = await _client.LoginAsync(email, password);

            if (result.Success)
            {
                Token = result.Data.token;
                AccountId = result.Data.account_id;

                // Persist token
                PlayerPrefs.SetString(_config.TokenStorageKey, Token);
                PlayerPrefs.SetString(_config.AccountIdStorageKey, AccountId);
                PlayerPrefs.Save();

                Debug.Log($"[AuthSessionManager] Login success: {AccountId} (expires: {result.Data.expires_at})");
                callback?.Invoke(true, null);
            }
            else
            {
                Debug.LogWarning($"[AuthSessionManager] Login failed: {result.ErrorMessage}");
                callback?.Invoke(false, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Clear the current session (logout).
        /// </summary>
        public void Logout()
        {
            Token = null;
            AccountId = null;
            PlayerPrefs.DeleteKey(_config.TokenStorageKey);
            PlayerPrefs.DeleteKey(_config.AccountIdStorageKey);
            PlayerPrefs.Save();
            Debug.Log("[AuthSessionManager] Logged out.");
        }

        /// <summary>
        /// Get the token for use in game server connection payload.
        /// Returns null if not authenticated or auth mode disabled.
        /// </summary>
        public string GetConnectionToken()
        {
            if (!AuthModeEnabled)
            {
                Debug.Log("[AuthSessionManager] Auth mode disabled - returning null token (dev bypass).");
                return null;
            }

            return Token;
        }
    }
}
