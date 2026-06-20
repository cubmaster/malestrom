using IronExiles.Combat;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace IronExiles.Networking
{
    public static class NetworkPlayerShipFactory
    {
        const string PrefabRootName = "NetworkPlayerShipPrefab";

        public static GameObject CreatePrefab()
        {
            var existing = GameObject.Find(PrefabRootName);
            if (existing != null)
            {
                return existing;
            }

            var ship = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ship.name = PrefabRootName;
            ship.transform.localScale = new Vector3(2f, 0.5f, 1f);
            ship.SetActive(false);
            Object.DontDestroyOnLoad(ship);

            var collider = ship.GetComponent<BoxCollider>();
            collider.size = Vector3.one;

            ship.AddComponent<NetworkObject>();
            ship.AddComponent<NetworkTransform>();
            ship.AddComponent<ShipInputController>();
            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(new Vector3(5000f, 5000f, 5000f));
            ship.AddComponent<NetworkShipMovementController>();
            var targetable = ship.AddComponent<TargetableEntity>();
            targetable.Configure("Player Ship", TargetAffiliation.Hostile, 100f);
            ship.AddComponent<NetworkShipTargetingController>();
            ship.AddComponent<ShipTargetInputController>();
            ship.AddComponent<NetworkedShipSetup>();

            return ship;
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
}
