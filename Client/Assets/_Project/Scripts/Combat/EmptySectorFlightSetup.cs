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
            SpawnTrainingDummy();
            var ship = CreateShip();
            AttachCockpitCamera(ship.transform);
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
            ship.AddComponent<LocalShipRadarSensor>();
            ship.AddComponent<ShipFlightTelemetryAdapter>();

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
