using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Spawns a placeholder player ship when EmptySector loads in Play Mode.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EmptySectorFlightSetup : MonoBehaviour
    {
        [SerializeField] Vector3 _spawnPosition = Vector3.zero;
        [SerializeField] Vector3 _sectorBoundsExtent = new Vector3(5000f, 5000f, 5000f);
        [SerializeField] Vector3 _hullScale = new Vector3(2f, 0.5f, 1f);

        void Start()
        {
            var ship = CreateShip();
            AttachCamera(ship.transform);
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

            ship.AddComponent<ShipInputController>();
            var movement = ship.AddComponent<ShipMovementController>();
            movement.SetSectorBoundsExtent(_sectorBoundsExtent);
            ship.AddComponent<ShipFlightTelemetryAdapter>();

            return ship;
        }

        void AttachCamera(Transform shipTransform)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var rig = camera.gameObject.GetComponent<ChaseCameraRig>();
            if (rig == null)
            {
                rig = camera.gameObject.AddComponent<ChaseCameraRig>();
            }

            rig.SetTarget(shipTransform);
        }
    }
}