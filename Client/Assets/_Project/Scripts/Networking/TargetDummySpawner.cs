using IronExiles.Combat;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class TargetDummyFactory
    {
        const string PrefabRootName = "NetworkTargetDummyPrefab";

        public static GameObject CreatePrefab()
        {
            var existing = GameObject.Find(PrefabRootName);
            if (existing != null)
            {
                return existing;
            }

            var dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummy.name = PrefabRootName;
            dummy.transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);
            dummy.SetActive(false);
            Object.DontDestroyOnLoad(dummy);

            var renderer = dummy.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.9f, 0.35f, 0.2f, 1f);
            }

            dummy.AddComponent<NetworkObject>();
            var targetable = dummy.AddComponent<TargetableEntity>();
            targetable.Configure("Training Dummy", TargetAffiliation.Neutral, 100f);

            return dummy;
        }

        public static void RegisterPrefab(GameObject prefab)
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null || prefab == null)
            {
                return;
            }

            foreach (var entry in networkManager.NetworkConfig.Prefabs.Prefabs)
            {
                if (entry.Prefab == prefab)
                {
                    return;
                }
            }

            networkManager.NetworkConfig.Prefabs.Add(new NetworkPrefab { Prefab = prefab });
        }
    }

    [DisallowMultipleComponent]
    public sealed class TargetDummySpawner : MonoBehaviour
    {
        [SerializeField] Vector3 _spawnPosition = new(40f, 0f, 40f);

        GameObject _dummyPrefab;
        bool _spawned;

        public void Configure(GameObject dummyPrefab, Vector3 spawnPosition)
        {
            _dummyPrefab = dummyPrefab;
            _spawnPosition = spawnPosition;
        }

        void OnEnable()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnServerStarted += OnServerStarted;
            TrySpawnDummy();
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
            TrySpawnDummy();
        }

        void TrySpawnDummy()
        {
            var networkManager = NetworkManager.Singleton;
            if (_spawned || _dummyPrefab == null || networkManager == null || !networkManager.IsServer)
            {
                return;
            }

            var instance = Instantiate(_dummyPrefab, _spawnPosition, Quaternion.identity);
            instance.SetActive(true);
            instance.GetComponent<NetworkObject>().Spawn();
            _spawned = true;
            Debug.Log($"[TargetDummySpawner] Spawned training dummy at {_spawnPosition}");
        }
    }
}
