using System.Collections;
using IronExiles.Combat;
using IronExiles.Combat.AI;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    /// <summary>
    /// Spawns configurable number of NPC ships on server start.
    /// Handles respawn after death with configurable delay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NPCSpawner : MonoBehaviour
    {
        [SerializeField] int _spawnCount = NPCSettings.DefaultSpawnCount;
        [SerializeField] float _spawnRadius = 150f;
        [SerializeField] Vector3 _spawnCenter = new(0f, 0f, 100f);

        GameObject _npcPrefab;
        bool _spawned;

        public void Configure(GameObject npcPrefab, int spawnCount = -1, Vector3? spawnCenter = null)
        {
            _npcPrefab = npcPrefab;
            if (spawnCount > 0) _spawnCount = spawnCount;
            if (spawnCenter.HasValue) _spawnCenter = spawnCenter.Value;
        }

        void OnEnable()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnServerStarted += OnServerStarted;
            TrySpawnAll();
        }

        void OnDisable()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnServerStarted -= OnServerStarted;
        }

        void OnServerStarted()
        {
            TrySpawnAll();
        }

        void TrySpawnAll()
        {
            var networkManager = NetworkManager.Singleton;
            if (_spawned || _npcPrefab == null || networkManager == null || !networkManager.IsServer)
            {
                return;
            }

            _spawned = true;

            for (var i = 0; i < _spawnCount; i++)
            {
                var position = ComputeSpawnPosition(i);
                SpawnNPC(position);
            }

            Debug.Log($"[NPCSpawner] Spawned {_spawnCount} NPC ships around {_spawnCenter}");
        }

        Vector3 ComputeSpawnPosition(int index)
        {
            // Distribute NPCs in a ring around spawn center
            var angle = (360f / _spawnCount) * index * Mathf.Deg2Rad;
            var offset = new Vector3(
                Mathf.Cos(angle) * _spawnRadius,
                Random.Range(-10f, 10f),
                Mathf.Sin(angle) * _spawnRadius);

            return _spawnCenter + offset;
        }

        void SpawnNPC(Vector3 position)
        {
            var instance = Instantiate(_npcPrefab, position, Quaternion.identity);
            instance.SetActive(true);

            var networkObject = instance.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError("[NPCSpawner] NPC prefab missing NetworkObject component.");
                Destroy(instance);
                return;
            }

            networkObject.Spawn();

            // Configure combat systems on server
            var health = instance.GetComponent<NetworkDamageableHealth>();
            if (health != null)
            {
                health.ConfigureForServer(NPCSettings.MaxHull);
                health.Destroyed += _ => OnNPCDestroyed(position);
            }

            var shields = instance.GetComponent<NetworkShipShieldController>();
            if (shields != null)
            {
                shields.ConfigureForServer(NPCSettings.ShieldPerFacing);
            }

            // Initialize AI
            var brain = instance.GetComponent<NPCBrain>();
            if (brain != null)
            {
                brain.Initialize(position, true);
            }

            var shipController = instance.GetComponent<NPCShipController>();
            if (shipController != null)
            {
                shipController.Initialize(true);
            }

            Debug.Log($"[NPCSpawner] NPC spawned at {position}");
        }

        void OnNPCDestroyed(Vector3 originalPosition)
        {
            StartCoroutine(RespawnAfterDelay(originalPosition));
        }

        IEnumerator RespawnAfterDelay(Vector3 position)
        {
            yield return new WaitForSeconds(NPCSettings.RespawnDelaySeconds);

            var networkManager = NetworkManager.Singleton;
            if (networkManager == null || !networkManager.IsServer)
            {
                yield break;
            }

            SpawnNPC(position);
            Debug.Log($"[NPCSpawner] NPC respawned at {position}");
        }
    }
}
