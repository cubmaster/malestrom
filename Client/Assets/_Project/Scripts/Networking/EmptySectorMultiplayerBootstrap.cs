using IronExiles.Combat;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace IronExiles.Networking
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class EmptySectorMultiplayerBootstrap : MonoBehaviour
    {
        [SerializeField] int _spawnPointCount = 4;
        [SerializeField] float _spawnSpacing = 20f;

        void Awake()
        {
            DisableLegacyFlightSetup();

            if (NetworkManager.Singleton != null)
            {
                return;
            }

            EnsureNetworkStack();
        }

        static void DisableLegacyFlightSetup()
        {
            foreach (var legacy in Object.FindObjectsByType<EmptySectorFlightSetup>(FindObjectsSortMode.None))
            {
                legacy.enabled = false;
            }
        }

        void EnsureNetworkStack()
        {
            var networkRoot = new GameObject("NetworkSession");
            networkRoot.AddComponent<NetworkManager>();
            networkRoot.AddComponent<UnityTransport>();
            networkRoot.AddComponent<NetworkSessionManager>();

            var spawnRoot = new GameObject("SpawnPointManager");
            var spawnManager = spawnRoot.AddComponent<SpawnPointManager>();
            spawnManager.ConfigureSpawnPoints(CreateSpawnPoints(spawnRoot.transform));

            var shipPrefab = NetworkPlayerShipFactory.CreatePrefab();
            NetworkPlayerShipFactory.RegisterPrefab(shipPrefab);

            var spawner = networkRoot.AddComponent<PlayerShipSpawner>();
            spawner.Configure(shipPrefab, spawnManager);

            Object.DontDestroyOnLoad(networkRoot);
            Object.DontDestroyOnLoad(spawnRoot);
        }

        Transform[] CreateSpawnPoints(Transform parent)
        {
            var points = new Transform[_spawnPointCount];
            for (var i = 0; i < _spawnPointCount; i++)
            {
                var point = new GameObject($"SpawnPoint_{i}");
                point.transform.SetParent(parent, false);
                point.transform.position = new Vector3(i * _spawnSpacing, 0f, 0f);
                points[i] = point.transform;
            }

            return points;
        }
    }
}
