using IronExiles.Combat;
using UnityEngine;

namespace IronExiles.UI
{
    public struct HardpointDisplayState
    {
        public string Label;
        public float ChargeFill01;
        public bool IsActive;
        public bool IsFiring;
        public HardpointType Type;
    }

    public struct FlightHudDisplayState
    {
        public string SpeedText;
        public string HeadingText;
        public float HullFill01;
        public float ShieldFill01;
        public float JumpChargeFill01;
        public bool JumpReady;
        public string JumpStatusText;
        public float PowerWeapons;
        public float PowerShields;
        public float PowerEngines;
        public float PowerEcm;
        public HardpointDisplayState[] Hardpoints;
        public int RadarContactCount;
        public Vector3[] RadarContacts;
        public string LockedTargetName;
        public float LockedTargetDistanceMeters;
        public float LockedTargetHullFill01;
        public ulong LockedTargetNetworkObjectId;
        public bool IsVisible;
    }

    public static class FlightHudPresenter
    {
        public static FlightHudDisplayState Build(IShipFlightTelemetry telemetry)
        {
            if (telemetry == null)
            {
                return new FlightHudDisplayState { IsVisible = false };
            }

            var power = telemetry.Power;
            var hardpoints = new HardpointDisplayState[telemetry.HardpointCount];
            for (int i = 0; i < telemetry.HardpointCount; i++)
            {
                var hp = telemetry.GetHardpoint(i);
                hardpoints[i] = new HardpointDisplayState
                {
                    Label = hp.Label,
                    ChargeFill01 = hp.ChargePercent / 100f,
                    IsActive = hp.IsActive,
                    IsFiring = hp.IsFiring,
                    Type = hp.Type
                };
            }

            var jumpCharge = telemetry.JumpDriveChargePercent;
            var jumpReady = telemetry.JumpDriveReady;

            Vector3[] radarContacts = null;
            if (telemetry.RadarContactCount > 0)
            {
                radarContacts = new Vector3[telemetry.RadarContactCount];
                for (var i = 0; i < telemetry.RadarContactCount; i++)
                {
                    radarContacts[i] = telemetry.GetRadarContact(i);
                }
            }

            return new FlightHudDisplayState
            {
                SpeedText = $"{telemetry.SpeedMetersPerSecond:F0} m/s",
                HeadingText = $"HDG {telemetry.HeadingDegrees:F0}°",
                HullFill01 = telemetry.HullPercent / 100f,
                ShieldFill01 = telemetry.ShieldPercent / 100f,
                JumpChargeFill01 = jumpCharge / 100f,
                JumpReady = jumpReady,
                JumpStatusText = jumpReady ? "JUMP RDY" : $"JUMP {jumpCharge:F0}%",
                PowerWeapons = power.Weapons,
                PowerShields = power.Shields,
                PowerEngines = power.Engines,
                PowerEcm = power.Ecm,
                Hardpoints = hardpoints,
                RadarContactCount = telemetry.RadarContactCount,
                RadarContacts = radarContacts,
                LockedTargetName = telemetry.LockedTargetName,
                LockedTargetDistanceMeters = telemetry.LockedTargetDistanceMeters,
                LockedTargetHullFill01 = telemetry.LockedTargetHullFill01,
                LockedTargetNetworkObjectId = telemetry.LockedTargetNetworkObjectId,
                IsVisible = true
            };
        }
    }
}
