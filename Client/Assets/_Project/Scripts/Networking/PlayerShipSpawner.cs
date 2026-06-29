using System.Collections;
using System.Linq;
using IronExiles.Combat;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    [DisallowMultipleComponent]
    public sealed class PlayerShipSpawner : MonoBehaviour
    {
        const float RespawnDelay = 5f;

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
            nm.OnClientDisconnectCallback += OnClientDisconnected;
        }

        void OnDisable()
        {
            var nm = NetworkManager.Singleton;
            if (nm == null)
            {
                return;
            }

            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        /// <summary>
        /// Subscribe to a ship's Destroyed event so the spawner can respawn
        /// the owning player after a delay. Call this on the server after spawning.
        /// </summary>
        public void RegisterForRespawn(NetworkObject shipNetworkObject)
        {
            if (shipNetworkObject == null)
            {
                return;
            }

            var health = shipNetworkObject.GetComponent<NetworkDamageableHealth>();
            if (health == null)
            {
                return;
            }

            var ownerClientId = shipNetworkObject.OwnerClientId;
            health.Destroyed += _ => RespawnPlayer(ownerClientId, RespawnDelay);
        }

        /// <summary>
        /// Respawns a player ship after the specified delay.
        /// Called by the destruction event system on the server.
        /// </summary>
        public void RespawnPlayer(ulong clientId, float delay)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            StartCoroutine(RespawnAfterDelay(clientId, delay));
        }

        IEnumerator RespawnAfterDelay(ulong clientId, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                yield break;
            }

            // Verify client is still connected
            if (!NetworkManager.Singleton.ConnectedClientsIds.Contains(clientId))
            {
                Debug.Log($"[PlayerShipSpawner] Client {clientId} disconnected before respawn.");
                yield break;
            }

            SpawnShipForClient(clientId);
        }

        void OnClientConnected(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            SpawnShipForClient(clientId);
        }

        void SpawnShipForClient(ulong clientId)
        {
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
            instance.SetActive(true);
            var networkObject = instance.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
                RegisterForRespawn(networkObject);
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
