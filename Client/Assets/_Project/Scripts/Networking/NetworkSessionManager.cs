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

        void Start()
        {
            if (!_autoStart)
            {
                return;
            }

            if (IsServerBuild())
            {
                StartServer();
            }
            else
            {
                StartClient();
            }
        }

        public void StartServer()
        {
            var port = NetworkCommandLineArgs.GetServerPort();
            ConfigureTransport("0.0.0.0", port);

            var nm = NetworkManager.Singleton;
            nm.ConnectionApprovalCallback = ApproveConnection;
            nm.StartServer();
            Debug.Log($"[NetworkSessionManager] Server started on port {port} (max {_maxConnections} players)");
        }

        public void StartClient()
        {
            var address = NetworkCommandLineArgs.GetConnectAddress();
            var port = NetworkCommandLineArgs.GetConnectPort();
            ConfigureTransport(address, port);

            NetworkManager.Singleton.StartClient();
            Debug.Log($"[NetworkSessionManager] Client connecting to {address}:{port}");
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
#if UNITY_SERVER
            return true;
#else
            return Application.isBatchMode;
#endif
        }
    }
}
