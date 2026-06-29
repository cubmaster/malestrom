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
                RuntimeNetworkPrefabUtility.EnsurePlayerShipPrefabHash(existing.GetComponent<NetworkObject>());
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
            RuntimeNetworkPrefabUtility.EnsurePlayerShipPrefabHash(ship.GetComponent<NetworkObject>());
            ship.AddComponent<NetworkTransform>();
            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(new Vector3(5000f, 5000f, 5000f));
            ship.AddComponent<ShipInputController>();
            ship.AddComponent<NetworkShipMovementController>();
            ship.AddComponent<NetworkShipReactorPowerController>();
            var targetable = ship.AddComponent<TargetableEntity>();
            targetable.Configure("Player Ship", TargetAffiliation.Hostile, 100f);
            ship.AddComponent<NetworkDamageableHealth>();
            ship.AddComponent<NetworkShipTargetingController>();
            ship.AddComponent<ShipTargetInputController>();
            ship.AddComponent<NetworkShipBeamWeaponController>();
            ship.AddComponent<ShipBeamWeaponInputController>();
            ship.AddComponent<BeamWeaponVfx>();
            ship.AddComponent<DestructionVfx>();
            ship.AddComponent<NetworkedShipSetup>();

            return ship;
        }

        public static void RegisterPrefab(GameObject prefab, NetworkManager networkManager = null)
        {
            RuntimeNetworkPrefabUtility.RegisterPrefab(prefab, networkManager ?? NetworkManager.Singleton);
        }
    }
}
