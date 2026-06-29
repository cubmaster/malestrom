using UnityEngine;
using UnityEngine.UIElements;

namespace IronExiles.Auth
{
    /// <summary>
    /// UI Toolkit-based login screen that gates multiplayer connect.
    /// Handles both registration and login flows.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LoginScreenController : MonoBehaviour
    {
        [SerializeField] AuthSessionManager _authSession;

        UIDocument _document;
        VisualElement _root;

        // Login form elements
        TextField _emailField;
        TextField _passwordField;
        Button _loginButton;
        Button _registerButton;
        Button _connectButton;
        Label _statusLabel;
        VisualElement _loginPanel;
        VisualElement _connectedPanel;

        void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            _root = _document.rootVisualElement;

            // Bind UI elements
            _emailField = _root.Q<TextField>("email-field");
            _passwordField = _root.Q<TextField>("password-field");
            _loginButton = _root.Q<Button>("login-button");
            _registerButton = _root.Q<Button>("register-button");
            _connectButton = _root.Q<Button>("connect-button");
            _statusLabel = _root.Q<Label>("status-label");
            _loginPanel = _root.Q<VisualElement>("login-panel");
            _connectedPanel = _root.Q<VisualElement>("connected-panel");

            // Wire callbacks
            if (_loginButton != null) _loginButton.clicked += OnLoginClicked;
            if (_registerButton != null) _registerButton.clicked += OnRegisterClicked;
            if (_connectButton != null) _connectButton.clicked += OnConnectClicked;

            // Check if already authenticated
            if (_authSession != null && _authSession.IsAuthenticated)
            {
                ShowConnectedState();
            }
            else
            {
                ShowLoginState();
            }
        }

        void OnDisable()
        {
            if (_loginButton != null) _loginButton.clicked -= OnLoginClicked;
            if (_registerButton != null) _registerButton.clicked -= OnRegisterClicked;
            if (_connectButton != null) _connectButton.clicked -= OnConnectClicked;
        }

        void OnLoginClicked()
        {
            var email = _emailField?.value;
            var password = _passwordField?.value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SetStatus("Please enter email and password.", Color.yellow);
                return;
            }

            SetStatus("Logging in...", Color.white);
            SetButtonsEnabled(false);

            _authSession.Login(email, password, (success, error) =>
            {
                SetButtonsEnabled(true);
                if (success)
                {
                    SetStatus("Login successful!", Color.green);
                    ShowConnectedState();
                }
                else
                {
                    SetStatus(error ?? "Login failed.", Color.red);
                }
            });
        }

        void OnRegisterClicked()
        {
            var email = _emailField?.value;
            var password = _passwordField?.value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SetStatus("Please enter email and password.", Color.yellow);
                return;
            }

            if (password.Length < 8)
            {
                SetStatus("Password must be at least 8 characters.", Color.yellow);
                return;
            }

            SetStatus("Registering...", Color.white);
            SetButtonsEnabled(false);

            _authSession.Register(email, password, (success, error) =>
            {
                SetButtonsEnabled(true);
                if (success)
                {
                    SetStatus("Registration successful! You can now log in.", Color.green);
                }
                else
                {
                    SetStatus(error ?? "Registration failed.", Color.red);
                }
            });
        }

        void OnConnectClicked()
        {
            // Trigger game server connection with auth token
            var sessionManager = FindFirstObjectByType<Networking.NetworkSessionManager>();
            if (sessionManager != null)
            {
                Debug.Log("[LoginScreen] Connecting to game server with auth token...");
                sessionManager.ConnectToServer();
            }
            else
            {
                SetStatus("NetworkSessionManager not found in scene.", Color.red);
            }
        }

        void ShowLoginState()
        {
            if (_loginPanel != null) _loginPanel.style.display = DisplayStyle.Flex;
            if (_connectedPanel != null) _connectedPanel.style.display = DisplayStyle.None;
        }

        void ShowConnectedState()
        {
            if (_loginPanel != null) _loginPanel.style.display = DisplayStyle.None;
            if (_connectedPanel != null) _connectedPanel.style.display = DisplayStyle.Flex;
        }

        void SetStatus(string message, Color color)
        {
            if (_statusLabel != null)
            {
                _statusLabel.text = message;
                _statusLabel.style.color = color;
            }
        }

        void SetButtonsEnabled(bool enabled)
        {
            if (_loginButton != null) _loginButton.SetEnabled(enabled);
            if (_registerButton != null) _registerButton.SetEnabled(enabled);
        }
    }
}
