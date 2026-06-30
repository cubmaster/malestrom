using IronExiles.Combat.AI;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class EmptySectorFlightSetup : MonoBehaviour
    {
        [SerializeField] Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] Vector3 _sectorBoundsExtent = new Vector3(5000f, 5000f, 5000f);
        [SerializeField] Vector3 _hullScale = new Vector3(2f, 0.5f, 1f);
        [SerializeField] Vector3 _trainingDummyPosition = new Vector3(40f, 0f, 40f);

        void Start()
        {
            ProceduralStarfieldEnvironment.Apply();
            SpawnSectorObjects();
            SpawnTrainingDummy();
            SpawnNPC(new Vector3(60f, 0f, 60f));
            var ship = CreateShip();
            AttachCockpitCamera(ship.transform);
        }

        void SpawnSectorObjects()
        {
            SectorObjectFactory.SpawnAll();
        }

        void SpawnTrainingDummy()
        {
            var dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummy.name = "TrainingDummy";
            dummy.transform.SetPositionAndRotation(_trainingDummyPosition, Quaternion.identity);
            dummy.transform.localScale = new Vector3(1.5f, 0.5f, 1.5f);

            var renderer = dummy.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.9f, 0.35f, 0.2f, 1f);
            }

            var targetable = dummy.AddComponent<TargetableEntity>();
            targetable.Configure("Training Dummy", TargetAffiliation.Neutral, 100f);
            targetable.AssignNetworkObjectIdForTests(9001UL);

            var health = dummy.AddComponent<NetworkDamageableHealth>();
            health.ConfigureForServer(100f); // Default 100 max hull
            dummy.AddComponent<DestructionVfx>();
        }

        void SpawnNPC(Vector3 position)
        {
            var npc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            npc.name = "KethariDrone";
            npc.transform.SetPositionAndRotation(position, Quaternion.identity);
            npc.transform.localScale = new Vector3(2f, 0.5f, 1.5f);

            var renderer = npc.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.7f, 0.15f, 0.1f, 1f);
            }

            var targetable = npc.AddComponent<TargetableEntity>();
            targetable.Configure("Kethari Drone", TargetAffiliation.Hostile, 100f);
            targetable.AssignNetworkObjectIdForTests(8000UL);

            npc.AddComponent<DamageableHull>().Configure(NPCSettings.MaxHull);
            npc.AddComponent<NetworkShipShieldController>();
            npc.AddComponent<NetworkShipTargetingController>();
            npc.AddComponent<NetworkShipBeamWeaponController>();
            npc.AddComponent<ShipMovementController>();

            var brain = npc.AddComponent<NPCBrain>();
            brain.Initialize(position, true);

            var controller = npc.AddComponent<NPCShipController>();
            controller.Initialize(true);
        }

        GameObject CreateShip()
        {
            var ship = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ship.name = "PlayerShip";
            ship.tag = "Player";
            ship.transform.SetPositionAndRotation(_spawnPosition, Quaternion.identity);
            ship.transform.localScale = _hullScale;

            var collider = ship.GetComponent<BoxCollider>();
            collider.size = Vector3.one;

            var renderer = ship.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(_sectorBoundsExtent);
            ship.AddComponent<ShipInputController>();
            ship.AddComponent<ShipReactorPowerController>();
            ship.AddComponent<NetworkShipShieldController>();
            ship.AddComponent<LocalShipRadarSensor>();

            var targetable = ship.AddComponent<TargetableEntity>();
            targetable.Configure("Player Ship", TargetAffiliation.Friendly, 100f);
            targetable.AssignNetworkObjectIdForTests(1000UL);

            ship.AddComponent<NetworkDamageableHealth>();
            ship.AddComponent<NetworkShipTargetingController>();
            ship.AddComponent<ShipTargetInputController>();
            ship.AddComponent<NetworkShipBeamWeaponController>();
            ship.AddComponent<ShipBeamWeaponInputController>();
            ship.AddComponent<BeamWeaponVfx>();

            ship.AddComponent<ShipFlightTelemetryAdapter>();
            ship.AddComponent<ShipWeaponsInputController>();

            LocalPlayerSystemsEvents.NotifyLocalPlayerShipReady(ship);

            return ship;
        }

        void AttachCockpitCamera(Transform shipTransform)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var rig = camera.gameObject.GetComponent<CockpitCameraRig>();
            if (rig == null)
            {
                rig = camera.gameObject.AddComponent<CockpitCameraRig>();
            }

            rig.SetTarget(shipTransform);
        }
    }
}
