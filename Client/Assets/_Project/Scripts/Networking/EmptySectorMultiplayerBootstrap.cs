using IronExiles.Combat;
using IronExiles.Combat.AI;
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
        [SerializeField] bool _autoConnectInEditor;

        void Awake()
        {
            ProceduralStarfieldEnvironment.Apply();

            if (NetworkManager.Singleton != null)
            {
                return;
            }

            var envValue = System.Environment.GetEnvironmentVariable("IRON_EXILES_AUTO_CONNECT");
            Debug.Log($"[EmptySectorMultiplayerBootstrap] IRON_EXILES_AUTO_CONNECT env = '{envValue}', ShouldAutoConnect = {LocalMultiplayerDevSettings.ShouldAutoConnectInEditor()}, _autoConnectInEditor = {_autoConnectInEditor}");

            if (LocalMultiplayerDevSettings.ShouldAutoConnectInEditor())
            {
                _autoConnectInEditor = true;
                Debug.Log("[EmptySectorMultiplayerBootstrap] Auto-connect enabled (local multiplayer dev).");
            }
            else if (!_autoConnectInEditor)
            {
                Debug.Log("[EmptySectorMultiplayerBootstrap] Auto-connect disabled; using offline flight setup.");
            }

#if UNITY_EDITOR
            if (!Application.isBatchMode && !_autoConnectInEditor)
            {
                EnableLegacyFlightSetup();
                return;
            }
#endif

            DisableLegacyFlightSetup();
            EnsureNetworkStack();
        }

        static void EnableLegacyFlightSetup()
        {
            foreach (var legacy in Object.FindObjectsByType<EmptySectorFlightSetup>(FindObjectsSortMode.None))
            {
                legacy.enabled = true;
            }
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
            SectorObjectFactory.SpawnAll();

            var networkRoot = new GameObject("NetworkSession");
            var networkManager = networkRoot.AddComponent<NetworkManager>();
            var transport = networkRoot.AddComponent<UnityTransport>();
            NetworkManagerConfigurator.EnsureInitialized(networkManager, transport);
            var sessionManager = networkRoot.AddComponent<NetworkSessionManager>();
            sessionManager.ConfigureEditorAutoConnect(_autoConnectInEditor);

            var spawnRoot = new GameObject("SpawnPointManager");
            var spawnManager = spawnRoot.AddComponent<SpawnPointManager>();
            spawnManager.ConfigureSpawnPoints(CreateSpawnPoints(spawnRoot.transform));

            var shipPrefab = NetworkPlayerShipFactory.CreatePrefab();
            NetworkPlayerShipFactory.RegisterPrefab(shipPrefab, networkManager);

            var spawner = networkRoot.AddComponent<PlayerShipSpawner>();
            spawner.Configure(shipPrefab, spawnManager);
            networkRoot.AddComponent<LocalPlayerHudBridge>();

            var dummyPrefab = TargetDummyFactory.CreatePrefab();
            TargetDummyFactory.RegisterPrefab(dummyPrefab, networkManager);
            var dummySpawner = networkRoot.AddComponent<TargetDummySpawner>();
            dummySpawner.Configure(dummyPrefab, new Vector3(40f, 0f, 40f));

            // NPC combat ships (REQ-041)
            var npcPrefab = NPCShipFactory.CreatePrefab();
            NPCShipFactory.RegisterPrefab(npcPrefab, networkManager);
            var npcSpawner = networkRoot.AddComponent<NPCSpawner>();
            npcSpawner.Configure(npcPrefab, NPCSettings.DefaultSpawnCount, new Vector3(0f, 0f, 150f));

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
