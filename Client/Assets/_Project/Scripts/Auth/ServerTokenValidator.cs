using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace IronExiles.Auth
{
    /// <summary>
    /// Server-side token validation.
    /// Validates JWT tokens by calling the Auth Service /validate endpoint.
    /// Used in the dedicated server's connection approval callback.
    ///
    /// BR-4: Game server rejects connect without valid token when auth mode enabled.
    ///       Flag to bypass for dev (AUTH_MODE_ENABLED env var or AuthConfig.AuthModeEnabled).
    /// </summary>
    public class ServerTokenValidator
    {
        readonly string _authServiceUrl;
        readonly bool _authModeEnabled;

        public ServerTokenValidator(string authServiceUrl, bool authModeEnabled)
        {
            _authServiceUrl = authServiceUrl.TrimEnd('/');
            _authModeEnabled = authModeEnabled;
        }

        /// <summary>
        /// Validate a connection token. Returns account_id if valid, null if invalid.
        /// When auth mode is disabled, always returns a bypass account ID.
        /// </summary>
        public async Task<TokenValidationResult> ValidateAsync(string token)
        {
            if (!_authModeEnabled)
            {
                Debug.Log("[ServerTokenValidator] Auth mode DISABLED - allowing connection (dev bypass).");
                return new TokenValidationResult
                {
                    IsValid = true,
                    AccountId = "dev-bypass",
                    Reason = "auth_disabled",
                };
            }

            if (string.IsNullOrEmpty(token))
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Reason = "no_token",
                };
            }

            try
            {
                var url = _authServiceUrl + "/validate";
                var payload = $"{{\"token\":\"{token}\"}}";
                var bodyBytes = Encoding.UTF8.GetBytes(payload);

                using var request = new UnityWebRequest(url, "POST");
                request.uploadHandler = new UploadHandlerRaw(bodyBytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.timeout = 5;

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogWarning($"[ServerTokenValidator] Auth service unreachable: {request.error}");
                    return new TokenValidationResult
                    {
                        IsValid = false,
                        Reason = "auth_service_unreachable",
                    };
                }

                var responseText = request.downloadHandler.text;

                if (request.responseCode == 200)
                {
                    var response = JsonUtility.FromJson<ValidateResponse>(responseText);
                    return new TokenValidationResult
                    {
                        IsValid = true,
                        AccountId = response.account_id,
                        Reason = "valid",
                    };
                }

                return new TokenValidationResult
                {
                    IsValid = false,
                    Reason = request.responseCode == 401 ? "token_invalid_or_expired" : "validation_error",
                };
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ServerTokenValidator] Exception during validation: {ex.Message}");
                return new TokenValidationResult
                {
                    IsValid = false,
                    Reason = "exception",
                };
            }
        }
    }

    public class TokenValidationResult
    {
        public bool IsValid;
        public string AccountId;
        public string Reason;
    }
}
