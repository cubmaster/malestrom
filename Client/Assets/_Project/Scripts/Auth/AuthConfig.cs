using UnityEngine;

namespace IronExiles.Auth
{
    /// <summary>
    /// Configuration for the Auth Service connection.
    /// Attach to a persistent GameObject or use ScriptableObject in production.
    /// </summary>
    [CreateAssetMenu(fileName = "AuthConfig", menuName = "IronExiles/Auth Config")]
    public class AuthConfig : ScriptableObject
    {
        [Header("Auth Service")]
        [Tooltip("Base URL of the auth service (no trailing slash)")]
        [SerializeField] string _authServiceUrl = "http://localhost:3000";

        [Header("Token Settings")]
        [Tooltip("PlayerPrefs key for storing the auth token")]
        [SerializeField] string _tokenStorageKey = "iron_exiles_auth_token";

        [Tooltip("PlayerPrefs key for storing the account ID")]
        [SerializeField] string _accountIdStorageKey = "iron_exiles_account_id";

        [Header("Auth Mode (BR-4)")]
        [Tooltip("When false, game server skips token validation (dev bypass)")]
        [SerializeField] bool _authModeEnabled = true;

        public string AuthServiceUrl => _authServiceUrl;
        public string TokenStorageKey => _tokenStorageKey;
        public string AccountIdStorageKey => _accountIdStorageKey;
        public bool AuthModeEnabled => _authModeEnabled;
    }
}
