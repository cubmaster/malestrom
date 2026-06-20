using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    [DisallowMultipleComponent]
    public sealed class PlayerShipSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _playerShipPrefab;
        [SerializeField] SpawnPointManager _spawnPointManager;

        public void Configure(GameObject playerShipPrefab, SpawnPointManager spawnPointManager)
        {
            _playerShipPrefab = playerShipPrefab;
            _spawnPointManager = spawnPointManager;
        }

        void OnEnable()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                return;
            }

            nm.OnClientConnectedCallback += OnClientConnected;
            nm.OnClientDisconnectedCallback += OnClientDisconnected;
        }

        void OnDisable()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                return;
            }

            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectedCallback -= OnClientDisconnected;
        }

        void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (_playerShipPrefab == null)
            {
                Debug.LogError("[PlayerShipSpawner] Player ship prefab is not assigned.");
                return;
            }

            var position = _spawnPointManager != null
                ? _spawnPointManager.GetNextSpawnPosition()
                : Vector3.zero;
            var rotation = _spawnPointManager != null
                ? _spawnPointManager.GetNextSpawnRotation()
                : Quaternion.identity;

            var instance = Instantiate(_playerShipPrefab, position, rotation);
            var networkObject = instance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                Debug.Log($"[PlayerShipSpawner] Spawned ship for client {clientId} at {position}");
            }
            else
            {
                Debug.LogError("[PlayerShipSpawner] Prefab missing NetworkObject component.");
                Destroy(instance);
            }
        }

        void OnClientDisconnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            var playerObject = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId);
            if (playerObject != null)
            {
                playerObject.Despawn(true);
                Debug.Log($"[PlayerShipSpawner] Despawned ship for disconnected client {clientId}");
            }
        }
    }
}
