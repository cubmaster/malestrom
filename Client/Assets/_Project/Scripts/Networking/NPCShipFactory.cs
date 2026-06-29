using IronExiles.Combat;
using IronExiles.Combat.AI;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    /// <summary>
    /// Creates NPC ship prefabs at runtime for network spawning.
    /// Follows the same pattern as TargetDummyFactory and NetworkPlayerShipFactory.
    /// </summary>
    public static class NPCShipFactory
    {
        const string PrefabRootName = "NetworkNPCShipPrefab";

        public static GameObject CreatePrefab()
        {
            var existing = GameObject.Find(PrefabRootName);
            if (existing != null)
            {
                EnsureNPCPrefabHash(existing.GetComponent<NetworkObject>());
                return existing;
            }

            var npc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            npc.name = PrefabRootName;
            npc.transform.localScale = new Vector3(2f, 0.5f, 1.5f);
            npc.SetActive(false);
            Object.DontDestroyOnLoad(npc);

            // Visual: dark red hostile ship
            var renderer = npc.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.7f, 0.15f, 0.1f, 1f);
            }

            // Networking
            npc.AddComponent<NetworkObject>();
            EnsureNPCPrefabHash(npc.GetComponent<NetworkObject>());

            // Combat systems (same as player per BR-2)
            var targetable = npc.AddComponent<TargetableEntity>();
            targetable.Configure("Kethari Drone", TargetAffiliation.Hostile, 100f);

            npc.AddComponent<NetworkDamageableHealth>();
            npc.AddComponent<NetworkShipShieldController>();
            npc.AddComponent<NetworkShipTargetingController>();
            npc.AddComponent<NetworkShipBeamWeaponController>();

            // Movement (ShipInputController omitted - NPC is AI-driven via NPCShipController)
            npc.AddComponent<ShipMovementController>();

            // AI
            npc.AddComponent<NPCBrain>();
            npc.AddComponent<NPCShipController>();

            return npc;
        }

        public static void RegisterPrefab(GameObject prefab, NetworkManager networkManager = null)
        {
            RuntimeNetworkPrefabUtility.RegisterPrefab(prefab, networkManager ?? NetworkManager.Singleton);
        }

        static void EnsureNPCPrefabHash(NetworkObject networkObject)
        {
            RuntimeNetworkPrefabUtility.EnsureNPCShipPrefabHash(networkObject);
        }
    }
}
