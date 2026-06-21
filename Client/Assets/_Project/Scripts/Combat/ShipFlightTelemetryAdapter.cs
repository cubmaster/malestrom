using System.Collections.Generic;
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
        string LockedTargetName { get; }
        float LockedTargetDistanceMeters { get; }
        float LockedTargetHullFill01 { get; }
        ulong LockedTargetNetworkObjectId { get; }
        bool IsActive { get; }
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipFlightTelemetryAdapter : MonoBehaviour, IShipFlightTelemetry
    {
        ShipMovementController _movement;
        NetworkShipTargetingController _targeting;
        LocalShipRadarSensor _localRadar;
        IShipReactorPowerControl _reactorPower;
        RadarContact[] _radarContacts = System.Array.Empty<RadarContact>();

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

        void Awake()
        {
            _movement = GetComponent<ShipMovementController>();
            _targeting = GetComponent<NetworkShipTargetingController>();
            _localRadar = GetComponent<LocalShipRadarSensor>();
            _reactorPower = ResolveReactorPowerControl();
            _jumpChargeTimer = JumpChargeDuration;
        }

        static IShipReactorPowerControl ResolveReactorPowerControl(Component host)
        {
            var network = host.GetComponent<NetworkShipReactorPowerController>();
            if (network != null)
            {
                return network;
            }

            return host.GetComponent<ShipReactorPowerController>();
        }

        IShipReactorPowerControl ResolveReactorPowerControl() => ResolveReactorPowerControl(this);

        void Update()
        {
            if (_jumpChargeTimer < JumpChargeDuration)
            {
                _jumpChargeTimer += Time.deltaTime;
            }

            IReadOnlyList<RadarContact> contacts = null;
            if (_targeting != null && _targeting.ProvidesLocalRadar)
            {
                contacts = _targeting.GetRadarContacts();
            }
            else if (_localRadar != null)
            {
                contacts = _localRadar.GetRadarContacts();
            }

            if (contacts == null || contacts.Count == 0)
            {
                _radarContacts = System.Array.Empty<RadarContact>();
            }
            else
            {
                var copy = new RadarContact[contacts.Count];
                for (var i = 0; i < contacts.Count; i++)
                {
                    copy[i] = contacts[i];
                }

                _radarContacts = copy;
            }
        }

        public float SpeedMetersPerSecond
        {
            get
            {
                var networkMovement = GetComponent<NetworkShipMovementController>();
                if (networkMovement != null && networkMovement.IsOwner)
                {
                    return networkMovement.PredictedSpeedMetersPerSecond;
                }

                return _movement != null ? _movement.Velocity.magnitude : 0f;
            }
        }

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

        public PowerAllocation Power => _reactorPower != null
            ? _reactorPower.Current
            : ReactorPowerAllocationMath.CombatPreset;

        public int HardpointCount => _hardpoints.Length;
        public HardpointStatus GetHardpoint(int index) => _hardpoints[index];

        public int RadarContactCount => _radarContacts.Length;

        public Vector3 GetRadarContact(int index)
        {
            if (index < 0 || index >= _radarContacts.Length)
            {
                return Vector3.zero;
            }

            var contact = _radarContacts[index];
            return new Vector3(contact.RadarPlane01.x, contact.RadarPlane01.y, contact.Distance);
        }

        public string LockedTargetName
        {
            get
            {
                var locked = _targeting != null ? _targeting.GetLockedTarget() : null;
                return locked != null ? locked.DisplayName : string.Empty;
            }
        }

        public float LockedTargetDistanceMeters
        {
            get
            {
                var locked = _targeting != null ? _targeting.GetLockedTarget() : null;
                return locked != null ? Vector3.Distance(transform.position, locked.transform.position) : 0f;
            }
        }

        public float LockedTargetHullFill01
        {
            get
            {
                var locked = _targeting != null ? _targeting.GetLockedTarget() : null;
                return locked != null ? locked.HullPercent / 100f : 0f;
            }
        }

        public ulong LockedTargetNetworkObjectId =>
            _targeting != null ? _targeting.LockedTargetNetworkObjectId : 0UL;

        public bool IsActive => isActiveAndEnabled && _movement != null;
    }
}
