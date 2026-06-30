using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace IronExiles.Auth
{
    /// <summary>
    /// HTTP client for the Iron Exiles Auth Service.
    /// Handles register, login, and token validation.
    /// </summary>
    public class AuthClient
    {
        readonly string _baseUrl;

        public AuthClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        /// <summary>
        /// Register a new account.
        /// </summary>
        public async Task<AuthResult<RegisterResponse>> RegisterAsync(string email, string password)
        {
            var payload = JsonUtility.ToJson(new RegisterRequest { email = email, password = password });
            return await PostAsync<RegisterResponse>("/register", payload);
        }

        /// <summary>
        /// Login with email and password. Returns JWT on success.
        /// </summary>
        public async Task<AuthResult<LoginResponse>> LoginAsync(string email, string password)
        {
            var payload = JsonUtility.ToJson(new LoginRequest { email = email, password = password });
            return await PostAsync<LoginResponse>("/login", payload);
        }

        /// <summary>
        /// Validate a JWT token (used by game server).
        /// </summary>
        public async Task<AuthResult<ValidateResponse>> ValidateTokenAsync(string token)
        {
            var payload = JsonUtility.ToJson(new ValidateRequest { token = token });
            return await PostAsync<ValidateResponse>("/validate", payload);
        }

        async Task<AuthResult<T>> PostAsync<T>(string endpoint, string jsonPayload)
        {
            var url = _baseUrl + endpoint;
            var bodyBytes = Encoding.UTF8.GetBytes(jsonPayload);

            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(bodyBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogWarning($"[AuthClient] Network error on {endpoint}: {request.error}");
                return new AuthResult<T>
                {
                    Success = false,
                    ErrorMessage = $"Network error: {request.error}",
                    StatusCode = 0,
                };
            }

            var responseText = request.downloadHandler.text;
            var statusCode = (int)request.responseCode;

            if (statusCode >= 200 && statusCode < 300)
            {
                var data = JsonUtility.FromJson<T>(responseText);
                return new AuthResult<T>
                {
                    Success = true,
                    Data = data,
                    StatusCode = statusCode,
                };
            }

            // Parse error response
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(responseText);
            return new AuthResult<T>
            {
                Success = false,
                ErrorMessage = string.IsNullOrEmpty(errorResponse.message) ? "Unknown error" : errorResponse.message,
                ErrorCode = string.IsNullOrEmpty(errorResponse.error) ? "unknown" : errorResponse.error,
                StatusCode = statusCode,
            };
        }

        // Request/Response DTOs (serialized with JsonUtility)

        [Serializable]
        struct RegisterRequest
        {
            public string email;
            public string password;
        }

        [Serializable]
        struct LoginRequest
        {
            public string email;
            public string password;
        }

        [Serializable]
        struct ValidateRequest
        {
            public string token;
        }

        [Serializable]
        struct ErrorResponse
        {
            public string error;
            public string message;
        }
    }

    /// <summary>
    /// Generic result wrapper for auth operations.
    /// </summary>
    public class AuthResult<T>
    {
        public bool Success;
        public T Data;
        public string ErrorMessage;
        public string ErrorCode;
        public int StatusCode;
    }

    [Serializable]
    public class RegisterResponse
    {
        public string account_id;
        public string email;
        public string created_at;
    }

    [Serializable]
    public class LoginResponse
    {
        public string token;
        public string account_id;
        public string expires_at;
    }

    [Serializable]
    public class ValidateResponse
    {
        public bool valid;
        public string account_id;
        public string email;
        public string expires_at;
    }
}
