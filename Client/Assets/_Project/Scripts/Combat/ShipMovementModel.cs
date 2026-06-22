using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Pure Newtonian 6DOF movement integrator (testable without Play Mode).
    /// Thrust applies acceleration; velocity persists in space with no artificial cap.
    /// </summary>
    public sealed class ShipMovementModel
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; } = Quaternion.identity;
        public Vector3 Velocity { get; private set; }

        public Vector3 SectorBoundsExtent { get; private set; } = new Vector3(5000f, 5000f, 5000f);

        ShipStatsSnapshot _stats = ShipStatsDefinition.HumanStarterFighterDefaults();
        float _enginePerformanceMultiplier = 1f;
        Vector3 _thrustInput;
        Vector3 _rotationInput;
        bool _brakeInput;

        public void Reset(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            Velocity = Vector3.zero;
            _thrustInput = Vector3.zero;
            _rotationInput = Vector3.zero;
            _brakeInput = false;
        }

        public void SetStats(ShipStatsSnapshot stats) => _stats = stats;

        public void SetEnginePerformanceMultiplier(float multiplier) =>
            _enginePerformanceMultiplier = Mathf.Max(0f, multiplier);

        public void SetSectorBoundsExtent(Vector3 extent)
        {
            SectorBoundsExtent = new Vector3(
                Mathf.Abs(extent.x),
                Mathf.Abs(extent.y),
                Mathf.Abs(extent.z));
        }

        public void SetMovementInput(Vector3 localThrustInput, Vector3 localRotationInput, bool brakeInput = false)
        {
            _thrustInput = new Vector3(
                Mathf.Clamp(localThrustInput.x, -1f, 1f),
                Mathf.Clamp(localThrustInput.y, -1f, 1f),
                Mathf.Clamp(localThrustInput.z, -1f, 1f));

            _rotationInput = new Vector3(
                Mathf.Clamp(localRotationInput.x, -1f, 1f),
                Mathf.Clamp(localRotationInput.y, -1f, 1f),
                Mathf.Clamp(localRotationInput.z, -1f, 1f));

            _brakeInput = brakeInput;
        }

        public void Simulate(float deltaTime)
        {
            if (deltaTime <= Mathf.Epsilon)
            {
                return;
            }

            PerformMovement(deltaTime);
            ApplyRotation(deltaTime);
            ClampToSectorBounds();
        }

        void PerformMovement(float deltaTime)
        {
            var forward = Rotation * Vector3.forward;
            var right = Rotation * Vector3.right;
            var up = Rotation * Vector3.up;

            var thrustScale = _enginePerformanceMultiplier;
            var acceleration = forward * (_thrustInput.x * _stats.ForwardThrust * thrustScale)
                + right * (_thrustInput.y * _stats.StrafeThrust * thrustScale)
                + up * (_thrustInput.z * _stats.StrafeThrust * thrustScale);

            Velocity += acceleration * deltaTime;

            if (_brakeInput)
            {
                var speed = Velocity.magnitude;
                if (speed > 0f)
                {
                    var decelerationAmount = _stats.BrakeDeceleration * deltaTime;
                    if (decelerationAmount >= speed)
                    {
                        Velocity = Vector3.zero;
                    }
                    else
                    {
                        Velocity -= Velocity.normalized * decelerationAmount;
                    }
                }
            }

            Position += Velocity * deltaTime;
        }

        void ApplyRotation(float deltaTime)
        {
            var rate = _stats.RotationRate * deltaTime;
            var deltaRot = Quaternion.Euler(
                _rotationInput.x * rate,
                _rotationInput.y * rate,
                _rotationInput.z * rate);

            Rotation = Rotation * deltaRot;
        }

        void ClampToSectorBounds()
        {
            if (SectorBoundsExtent.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            var clamped = new Vector3(
                Mathf.Clamp(Position.x, -SectorBoundsExtent.x, SectorBoundsExtent.x),
                Mathf.Clamp(Position.y, -SectorBoundsExtent.y, SectorBoundsExtent.y),
                Mathf.Clamp(Position.z, -SectorBoundsExtent.z, SectorBoundsExtent.z));

            if (clamped == Position)
            {
                return;
            }

            if (!Mathf.Approximately(Position.x, clamped.x))
            {
                Velocity = new Vector3(0f, Velocity.y, Velocity.z);
            }

            if (!Mathf.Approximately(Position.y, clamped.y))
            {
                Velocity = new Vector3(Velocity.x, 0f, Velocity.z);
            }

            if (!Mathf.Approximately(Position.z, clamped.z))
            {
                Velocity = new Vector3(Velocity.x, Velocity.y, 0f);
            }

            Position = clamped;
        }
    }
}
