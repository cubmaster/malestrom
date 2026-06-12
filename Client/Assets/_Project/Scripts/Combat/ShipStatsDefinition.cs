using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Per-hull flight stats. Unity uses meters / m/s² (UE reference values are centimeters).
    /// </summary>
    [CreateAssetMenu(fileName = "ShipStats", menuName = "Iron Exiles/Ship Stats Definition")]
    public sealed class ShipStatsDefinition : ScriptableObject
    {
        [Min(0.01f)] public float MaxSpeed = 50f;
        [Min(0f)] public float ForwardThrust = 25f;
        [Min(0f)] public float StrafeThrust = 18f;
        [Min(0f)] public float RotationRate = 90f;
        [Min(0f)] public float BrakeDeceleration = 12f;

        public ShipStatsSnapshot ToSnapshot() => new ShipStatsSnapshot(
            MaxSpeed,
            ForwardThrust,
            StrafeThrust,
            RotationRate,
            BrakeDeceleration);

        public static ShipStatsSnapshot HumanStarterFighterDefaults() => new ShipStatsSnapshot(
            maxSpeed: 50f,
            forwardThrust: 25f,
            strafeThrust: 18f,
            rotationRate: 90f,
            brakeDeceleration: 12f);
    }

    public readonly struct ShipStatsSnapshot
    {
        public readonly float MaxSpeed;
        public readonly float ForwardThrust;
        public readonly float StrafeThrust;
        public readonly float RotationRate;
        public readonly float BrakeDeceleration;

        public ShipStatsSnapshot(
            float maxSpeed,
            float forwardThrust,
            float strafeThrust,
            float rotationRate,
            float brakeDeceleration)
        {
            MaxSpeed = maxSpeed;
            ForwardThrust = forwardThrust;
            StrafeThrust = strafeThrust;
            RotationRate = rotationRate;
            BrakeDeceleration = brakeDeceleration;
        }
    }
}
