using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace IronExiles.Networking
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkManager))]
    public sealed class NetworkSessionManager : MonoBehaviour
    {
        [SerializeField] ushort _defaultPort = NetworkCommandLineArgs.DefaultPort;
        [SerializeField] string _defaultAddress = NetworkCommandLineArgs.DefaultAddress;
        [SerializeField] int _maxConnections = 10;
        [SerializeField] bool _autoStart = true;
        [SerializeField] bool _autoConnectInEditor;
        [SerializeField] float _clientConnectTimeoutSeconds = 5f;

        void Start()
        {
            var networkManager = GetComponent<NetworkManager>();
            if (networkManager != null)
            {
                networkManager.OnTransportFailure += HandleTransportFailure;
            }

            if (!_autoStart)
            {
                return;
            }

            if (IsServerBuild())
            {
                StartServer();
                return;
            }

#if UNITY_EDITOR
            if (!Application.isBatchMode && !_autoConnectInEditor)
            {
                Debug.Log("[NetworkSessionManager] Editor play: auto-connect disabled. Start the dedicated server, then use Connect To Server on NetworkSessionManager (or enable Auto Connect In Editor on EmptySectorMultiplayerBootstrap).");
                return;
            }

            if (LocalMultiplayerDevSettings.ShouldAutoConnectInEditor())
            {
                _clientConnectTimeoutSeconds = Mathf.Max(_clientConnectTimeoutSeconds, 30f);
            }
#endif

            StartClient();
        }

        void OnDestroy()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager != null)
            {
                networkManager.OnTransportFailure -= HandleTransportFailure;
            }
        }

        void HandleTransportFailure()
        {
            Debug.LogWarning(
                "[NetworkSessionManager] Network transport failed. For local multiplayer, start the dedicated server first " +
                $"(see docs/local-multiplayer-test.md). Expected client target: {NetworkCommandLineArgs.GetConnectAddress()}:{NetworkCommandLineArgs.GetConnectPort()}");

            ShutdownNetworkManagerIfActive();
        }

        public void StartServer()
        {
            var port = NetworkCommandLineArgs.GetServerPort();
            var nm = NetworkManager.Singleton;
            NetworkManagerConfigurator.EnsureInitialized(nm, nm.GetComponent<UnityTransport>());
            ConfigureTransport("0.0.0.0", port);

            nm.ConnectionApprovalCallback = ApproveConnection;
            nm.StartServer();
            Debug.Log($"[NetworkSessionManager] Server started on port {port} (max {_maxConnections} players)");
        }

        public void ConfigureEditorAutoConnect(bool autoConnectInEditor)
        {
            _autoConnectInEditor = autoConnectInEditor || LocalMultiplayerDevSettings.ShouldAutoConnectInEditor();
        }

        [ContextMenu("Connect To Server")]
        public void ConnectToServer()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                Debug.LogError("[NetworkSessionManager] No NetworkManager — is EmptySectorMultiplayerBootstrap in the scene?");
                return;
            }

            if (nm.IsClient || nm.IsServer || nm.IsHost)
            {
                Debug.Log("[NetworkSessionManager] Already connected or connecting.");
                return;
            }

            StartClient();
        }

        public void StartClient()
        {
            var address = NetworkCommandLineArgs.GetConnectAddress();
            var port = NetworkCommandLineArgs.GetConnectPort();
            var nm = NetworkManager.Singleton;
            NetworkManagerConfigurator.EnsureInitialized(nm, nm.GetComponent<UnityTransport>());
            ConfigureTransport(address, port);

            nm.StartClient();
            Debug.Log($"[NetworkSessionManager] Client connecting to {address}:{port}");

            if (_clientConnectTimeoutSeconds > 0f)
            {
                StartCoroutine(ShutdownIfClientNeverConnects());
            }
        }

        IEnumerator ShutdownIfClientNeverConnects()
        {
            yield return new WaitForSeconds(_clientConnectTimeoutSeconds);

            var nm = NetworkManager.Singleton;
            if (nm == null || nm.ShutdownInProgress || !nm.IsClient || nm.IsConnectedClient)
            {
                yield break;
            }

            Debug.LogWarning(
                $"[NetworkSessionManager] Timed out connecting to {NetworkCommandLineArgs.GetConnectAddress()}:{NetworkCommandLineArgs.GetConnectPort()}. " +
                "Start the dedicated server (Scripts/Run-UnityDedicatedServer.ps1) before playing the client.");

            ShutdownNetworkManagerIfActive();
        }

        static void ShutdownNetworkManagerIfActive()
        {
            var nm = NetworkManager.Singleton;
            if (nm != null && !nm.ShutdownInProgress)
            {
                nm.Shutdown();
            }
        }

        void ConfigureTransport(string address, ushort port)
        {
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(address, port);
            }
        }

        void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var connectedCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
            response.Approved = connectedCount < _maxConnections;
            response.CreatePlayerObject = false;

            if (!response.Approved)
            {
                Debug.LogWarning($"[NetworkSessionManager] Connection rejected — server full ({connectedCount}/{_maxConnections})");
            }
        }

        static bool IsServerBuild()
        {
#if UNITY_EDITOR
            // Interactive Editor Play is always the game client. The dedicated server is a separate exe/process.
            if (!Application.isBatchMode)
            {
                return false;
            }
#endif

#if UNITY_SERVER
            return true;
#else
            return NetworkCommandLineArgs.IsDedicatedServerLaunch();
#endif
        }
    }

    internal static class NetworkManagerConfigurator
    {
        public static void EnsureInitialized(NetworkManager networkManager, UnityTransport transport = null)
        {
            if (networkManager == null)
            {
                return;
            }

            transport ??= networkManager.GetComponent<UnityTransport>();

            if (networkManager.NetworkConfig == null)
            {
                networkManager.NetworkConfig = new NetworkConfig();
            }

            if (transport != null)
            {
                networkManager.NetworkConfig.NetworkTransport = transport;
            }

            networkManager.NetworkConfig.ConnectionApproval = true;
            networkManager.NetworkConfig.Prefabs.Initialize();
        }
    }
}
