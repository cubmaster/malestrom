using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    internal static class RuntimeNetworkPrefabUtility
    {
        const string PlayerShipPrefabId = "IronExiles.NetworkPlayerShip.v1";
        const string TargetDummyPrefabId = "IronExiles.NetworkTargetDummy.v1";

        static readonly FieldInfo GlobalObjectIdHashField = typeof(NetworkObject).GetField(
            "GlobalObjectIdHash",
            BindingFlags.Instance | BindingFlags.NonPublic);

        public static void EnsurePlayerShipPrefabHash(NetworkObject networkObject)
        {
            EnsureRuntimePrefabHash(networkObject, PlayerShipPrefabId);
        }

        public static void EnsureTargetDummyPrefabHash(NetworkObject networkObject)
        {
            EnsureRuntimePrefabHash(networkObject, TargetDummyPrefabId);
        }

        public static void RegisterPrefab(GameObject prefab, NetworkManager networkManager)
        {
            if (networkManager == null || prefab == null)
            {
                return;
            }

            NetworkManagerConfigurator.EnsureInitialized(networkManager);

            var networkObject = prefab.GetComponent<NetworkObject>();
            if (networkObject == null)
            {
                Debug.LogError($"[RuntimeNetworkPrefabUtility] Prefab '{prefab.name}' is missing NetworkObject.");
                return;
            }

            if (networkObject.PrefabIdHash == 0)
            {
                Debug.LogError(
                    $"[RuntimeNetworkPrefabUtility] Prefab '{prefab.name}' has GlobalObjectIdHash 0. Assign a stable hash before registration.");
                return;
            }

            var prefabs = networkManager.NetworkConfig.Prefabs.Prefabs;
            foreach (var entry in prefabs)
            {
                if (entry.Prefab == prefab)
                {
                    return;
                }
            }

            networkManager.AddNetworkPrefab(prefab);
        }

        static void EnsureRuntimePrefabHash(NetworkObject networkObject, string stablePrefabId)
        {
            if (networkObject == null)
            {
                return;
            }

            if (networkObject.PrefabIdHash != 0)
            {
                return;
            }

            var hash = StablePrefabHash(stablePrefabId);
            if (hash == 0)
            {
                hash = 1;
            }

            GlobalObjectIdHashField?.SetValue(networkObject, hash);
        }

        static uint StablePrefabHash(string value)
        {
            unchecked
            {
                const uint offsetBasis = 2166136261;
                const uint prime = 16777619;
                var hash = offsetBasis;

                for (var i = 0; i < value.Length; i++)
                {
                    hash ^= value[i];
                    hash *= prime;
                }

                return hash;
            }
        }
    }
}
