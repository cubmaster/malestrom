using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Exposes ship movement state for HUD binding (REQ-034).
    /// </summary>
    public interface IShipFlightTelemetry
    {
        float SpeedMetersPerSecond { get; }
        float HeadingDegrees { get; }
        float HullPercent { get; }
        bool IsActive { get; }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipFlightTelemetryAdapter : MonoBehaviour, IShipFlightTelemetry
    {
        ShipMovementController _movement;

        void Awake() => _movement = GetComponent<ShipMovementController>();

        public float SpeedMetersPerSecond => _movement != null ? _movement.Velocity.magnitude : 0f;

        public float HeadingDegrees
        {
            get
            {
                if (_movement == null)
                {
                    return 0f;
                }

                var yaw = _movement.transform.rotation.eulerAngles.y;
                return yaw < 0f ? yaw + 360f : yaw;
            }
        }

        public float HullPercent => 100f;

        public bool IsActive => isActiveAndEnabled && _movement != null && _movement.isActiveAndEnabled;
    }
}
