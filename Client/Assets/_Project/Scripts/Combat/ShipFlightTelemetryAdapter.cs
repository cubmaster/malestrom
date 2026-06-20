using UnityEngine;

namespace IronExiles.Combat
{
    public enum HardpointType { Weapon, Missile, PointDefense, Shield }

    public struct HardpointStatus
    {
        public HardpointType Type;
        public string Label;
        public float ChargePercent;
        public bool IsActive;
        public bool IsFiring;
    }

    public struct PowerAllocation
    {
        public float Weapons;
        public float Shields;
        public float Engines;
        public float Ecm;
    }

    public interface IShipFlightTelemetry
    {
        float SpeedMetersPerSecond { get; }
        float HeadingDegrees { get; }
        float HullPercent { get; }
        float ShieldPercent { get; }
        float JumpDriveChargePercent { get; }
        bool JumpDriveReady { get; }
        PowerAllocation Power { get; }
        int HardpointCount { get; }
        HardpointStatus GetHardpoint(int index);
        int RadarContactCount { get; }
        Vector3 GetRadarContact(int index);
        bool IsActive { get; }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipFlightTelemetryAdapter : MonoBehaviour, IShipFlightTelemetry
    {
        ShipMovementController _movement;

        float _jumpChargeTimer;
        const float JumpChargeDuration = 15f;

        readonly HardpointStatus[] _hardpoints = new HardpointStatus[]
        {
            new HardpointStatus { Type = HardpointType.Weapon, Label = "RAIL 1", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.Weapon, Label = "RAIL 2", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.Weapon, Label = "BEAM 1", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.Missile, Label = "MSL A", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.Missile, Label = "MSL B", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.PointDefense, Label = "PD", ChargePercent = 100f, IsActive = true },
            new HardpointStatus { Type = HardpointType.Shield, Label = "SHLD", ChargePercent = 100f, IsActive = true },
        };

        PowerAllocation _power = new PowerAllocation
        {
            Weapons = 0.50f,
            Shields = 0.30f,
            Engines = 0.12f,
            Ecm = 0.08f
        };

        void Awake()
        {
            _movement = GetComponent<ShipMovementController>();
            _jumpChargeTimer = JumpChargeDuration;
        }

        void Update()
        {
            if (_jumpChargeTimer < JumpChargeDuration)
            {
                _jumpChargeTimer += Time.deltaTime;
            }
        }

        public float SpeedMetersPerSecond => _movement != null ? _movement.Velocity.magnitude : 0f;

        public float HeadingDegrees
        {
            get
            {
                if (_movement == null) return 0f;
                var yaw = _movement.transform.rotation.eulerAngles.y;
                return yaw < 0f ? yaw + 360f : yaw;
            }
        }

        public float HullPercent => 100f;
        public float ShieldPercent => 100f;

        public float JumpDriveChargePercent => Mathf.Clamp01(_jumpChargeTimer / JumpChargeDuration) * 100f;
        public bool JumpDriveReady => _jumpChargeTimer >= JumpChargeDuration;

        public PowerAllocation Power => _power;

        public int HardpointCount => _hardpoints.Length;
        public HardpointStatus GetHardpoint(int index) => _hardpoints[index];

        public int RadarContactCount => 0;
        public Vector3 GetRadarContact(int index) => Vector3.zero;

        public bool IsActive => isActiveAndEnabled && _movement != null && _movement.isActiveAndEnabled;
    }
}
