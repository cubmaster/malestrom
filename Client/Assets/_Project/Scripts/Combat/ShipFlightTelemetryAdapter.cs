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
        ulong GetRadarContactNetworkObjectId(int index);
        string LockedTargetName { get; }
        float LockedTargetDistanceMeters { get; }
        float LockedTargetHullFill01 { get; }
        ulong LockedTargetNetworkObjectId { get; }
        bool IsActive { get; }
        int ShieldFacingCount { get; }
        float GetShieldFacingPercent(int facingIndex);
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipFlightTelemetryAdapter : MonoBehaviour, IShipFlightTelemetry
    {
        ShipMovementController _movement;
        NetworkShipTargetingController _targeting;
        NetworkShipBeamWeaponController _beamWeapon;
        NetworkDamageableHealth _damageableHealth;
        NetworkShipShieldController _shieldController;
        LocalShipRadarSensor _localRadar;
        IShipReactorPowerControl _reactorPower;
        RadarContact[] _radarContacts = System.Array.Empty<RadarContact>();

        float _jumpChargeTimer;
        const float JumpChargeDuration = 15f;

        [System.Serializable]
        public class WeaponSlotData
        {
            public string Name;
            public HardpointType Type;
            public int Ammo;
            public int MaxAmmo = 50;
            public float RechargeTimer;
            public float MaxRecharge = 5f;
            public bool IsBeam;
        }

        readonly WeaponSlotData[] _weaponSlots = new WeaponSlotData[]
        {
            new WeaponSlotData { Name = "Rail 1", Type = HardpointType.Weapon, Ammo = 50, MaxAmmo = 50, IsBeam = false },
            new WeaponSlotData { Name = "Rail 2", Type = HardpointType.Weapon, Ammo = 50, MaxAmmo = 50, IsBeam = false },
            new WeaponSlotData { Name = "Beam 1", Type = HardpointType.Weapon, RechargeTimer = 5f, MaxRecharge = 5f, IsBeam = true },
            new WeaponSlotData { Name = "MSLA", Type = HardpointType.Missile, Ammo = 50, MaxAmmo = 50, IsBeam = false },
            new WeaponSlotData { Name = "MSLB", Type = HardpointType.Missile, Ammo = 50, MaxAmmo = 50, IsBeam = false },
        };

        bool _isBeamFiring;
        bool _isBeamActive;
        float _beamBurstTimer;

        public void EquipWeaponInSlot(int slotIndex, string name, HardpointType type, bool isBeam, int ammo = 50, float maxRecharge = 5f)
        {
            if (slotIndex < 0 || slotIndex >= _weaponSlots.Length) return;
            _weaponSlots[slotIndex] = new WeaponSlotData
            {
                Name = name,
                Type = type,
                Ammo = ammo,
                MaxAmmo = ammo,
                RechargeTimer = maxRecharge,
                MaxRecharge = maxRecharge,
                IsBeam = isBeam
            };
        }

        public void FireWeaponSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _weaponSlots.Length) return;
            var slot = _weaponSlots[slotIndex];
            if (slot.IsBeam)
            {
                return;
            }

            if (slot.Ammo <= 0)
            {
                Debug.LogWarning($"[Weapons] {slot.Name} has no ammo remaining!");
                return;
            }

            slot.Ammo--;
            Debug.Log($"[Weapons] Fired {slot.Name}. Ammo remaining: {slot.Ammo}");

            var lockedTarget = _targeting != null ? _targeting.GetLockedTarget() : null;
            var targetHealth = lockedTarget != null ? lockedTarget.GetComponent<NetworkDamageableHealth>() : null;
            var targetTransform = lockedTarget != null ? lockedTarget.transform : null;

            float damage = slot.Type == HardpointType.Missile ? 50f : 20f;
            var projType = slot.Type == HardpointType.Missile ? Projectile.ProjectileType.Missile : Projectile.ProjectileType.Rail;

            // Spawn from the camera's perspective so the player sees it fly out from their screen
            Vector3 spawnPos = transform.position + transform.forward * 2f;
            Vector3 fallbackDir = transform.forward;

            if (Camera.main != null)
            {
                spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
                fallbackDir = Camera.main.transform.forward;
            }

            Projectile.Create(projType, spawnPos, targetTransform, targetHealth, damage, fallbackDir);
        }

        public void SetBeamFiringActive(bool active)
        {
            _isBeamFiring = active;
        }

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
            _beamWeapon = GetComponent<NetworkShipBeamWeaponController>();
            _damageableHealth = GetComponent<NetworkDamageableHealth>();
            _shieldController = GetComponent<NetworkShipShieldController>();
            _localRadar = GetComponent<LocalShipRadarSensor>();
            _reactorPower = ResolveReactorPowerControl();
            _jumpChargeTimer = JumpChargeDuration;
            _isBeamFiring = false;
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

            // Process Beam Firing and Recharge
            var beamSlot = _weaponSlots[2]; // Slot 2 is Beam 1

            if (_isBeamFiring)
            {
                // Can only trigger if we are fully charged and not already active
                if (!_isBeamActive && beamSlot.RechargeTimer >= beamSlot.MaxRecharge)
                {
                    _isBeamActive = true;
                    beamSlot.RechargeTimer = 0f; // Reset timer to 0 on fire!
                    _beamBurstTimer = 1.0f; // Firing burst lasts for 1 second
                }
            }

            // Count up recharge timer if it's less than max recharge
            if (beamSlot.RechargeTimer < beamSlot.MaxRecharge)
            {
                beamSlot.RechargeTimer += Time.deltaTime;
                if (beamSlot.RechargeTimer > beamSlot.MaxRecharge)
                {
                    beamSlot.RechargeTimer = beamSlot.MaxRecharge;
                }
            }

            // Process active firing state
            if (_isBeamActive)
            {
                _beamBurstTimer -= Time.deltaTime;
                if (_beamBurstTimer <= 0f)
                {
                    _isBeamActive = false;
                }

                // Turn on/off beam weapon firing states
                if (_beamWeapon != null)
                {
                    if (_isBeamActive)
                    {
                        if (_beamWeapon.IsSpawned) _beamWeapon.SetFiringServerRpc(true);
                        else _beamWeapon.SetFiringOffline(true);
                    }
                    else
                    {
                        if (_beamWeapon.IsSpawned) _beamWeapon.SetFiringServerRpc(false);
                        else _beamWeapon.SetFiringOffline(false);
                    }
                }
            }
            else
            {
                // Ensure we don't leave the beam weapon firing if active is false
                if (_beamWeapon != null && _beamWeapon.IsFiring)
                {
                    if (_beamWeapon.IsSpawned) _beamWeapon.SetFiringServerRpc(false);
                    else _beamWeapon.SetFiringOffline(false);
                }
            }

            // Synchronize Weapon Slots state to Hardpoints array
            for (var i = 0; i < 5; i++)
            {
                var slot = _weaponSlots[i];
                var hp = _hardpoints[i];
                hp.Type = slot.Type;
                
                if (slot.IsBeam)
                {
                    hp.Label = $"{slot.Name.ToUpper()}: {slot.RechargeTimer:F1}s";
                    hp.ChargePercent = (slot.RechargeTimer / slot.MaxRecharge) * 100f;
                    hp.IsFiring = _beamWeapon != null && _beamWeapon.IsFiring;
                }
                else
                {
                    hp.Label = $"{slot.Name.ToUpper()}: {slot.Ammo}";
                    hp.ChargePercent = ((float)slot.Ammo / slot.MaxAmmo) * 100f;
                    hp.IsFiring = false;
                }
                _hardpoints[i] = hp;
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

        public float HullPercent => _damageableHealth != null ? _damageableHealth.HullPercent : 100f;

        public float ShieldPercent
        {
            get
            {
                if (_shieldController == null || _shieldController.MaxShieldPerFacing <= 0f)
                    return 100f;
                var state = _shieldController.Current;
                var total = state.Front + state.Rear + state.Port + state.Starboard;
                var max = _shieldController.MaxShieldPerFacing * 4f;
                return (total / max) * 100f;
            }
        }

        public int ShieldFacingCount => 4;

        public float GetShieldFacingPercent(int facingIndex)
        {
            if (_shieldController == null)
                return 100f;
            var facing = (ShieldFacing)facingIndex;
            return _shieldController.GetFacingPercent(facing);
        }

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

        public ulong GetRadarContactNetworkObjectId(int index)
        {
            if (index < 0 || index >= _radarContacts.Length)
            {
                return 0UL;
            }

            return _radarContacts[index].NetworkObjectId;
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
