using UnityEngine;

namespace IronExiles.Combat.AI
{
    /// <summary>
    /// Translates NPCBrain decisions into movement commands for the ship.
    /// Runs server-side only, feeding synthetic input to ShipMovementController.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NPCBrain))]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class NPCShipController : MonoBehaviour
    {
        NPCBrain _brain;
        ShipMovementController _movement;
        bool _isServerActive;

        public void Initialize(bool isServer)
        {
            _isServerActive = isServer;
            _brain = GetComponent<NPCBrain>();
            _movement = GetComponent<ShipMovementController>();

            if (_isServerActive)
            {
                // Prevent ShipMovementController.Update from running its own input loop;
                // we drive movement explicitly via SimulateInput.
                _movement.SetNetworkDriverActive(true);
            }
        }

        void Update()
        {
            if (!_isServerActive)
            {
                return;
            }

            var state = _brain.CurrentState;
            switch (state)
            {
                case NPCBrainState.Patrol:
                    UpdatePatrolMovement();
                    break;
                case NPCBrainState.Combat:
                    UpdateCombatMovement();
                    break;
                case NPCBrainState.Dead:
                case NPCBrainState.Idle:
                    ApplyBrake();
                    break;
            }
        }

        void UpdatePatrolMovement()
        {
            var waypoint = _brain.CurrentWaypoint;
            var toWaypoint = waypoint - transform.position;
            var distance = toWaypoint.magnitude;

            if (distance < 5f)
            {
                // Close enough to waypoint, drift
                ApplyBrake();
                return;
            }

            var desiredDirection = toWaypoint.normalized;
            var input = ComputeSteeringInput(desiredDirection, NPCSettings.PatrolSpeedFraction);
            _movement.SimulateInput(input, Time.deltaTime);
            transform.SetPositionAndRotation(_movement.Model.Position, _movement.Model.Rotation);
        }

        void UpdateCombatMovement()
        {
            var target = _brain.CurrentTarget;
            if (target == null)
            {
                ApplyBrake();
                return;
            }

            var toTarget = target.position - transform.position;
            var distance = toTarget.magnitude;
            var desiredDirection = toTarget.normalized;

            float thrustFraction;
            if (distance > NPCSettings.EngagementDistance * 1.2f)
            {
                // Approach target
                thrustFraction = NPCSettings.CombatSpeedFraction;
            }
            else if (distance < NPCSettings.EngagementDistance * 0.5f)
            {
                // Too close, back off (negative thrust via rotation away)
                thrustFraction = -NPCSettings.PatrolSpeedFraction;
            }
            else
            {
                // In engagement zone, maintain position
                thrustFraction = 0.1f;
            }

            var input = ComputeSteeringInput(desiredDirection, thrustFraction);
            _movement.SimulateInput(input, Time.deltaTime);
            transform.SetPositionAndRotation(_movement.Model.Position, _movement.Model.Rotation);
        }

        ShipMovementInput ComputeSteeringInput(Vector3 worldDesiredDirection, float thrustFraction)
        {
            // Compute rotation toward desired direction
            var currentForward = transform.forward;
            var rotationNeeded = Quaternion.FromToRotation(currentForward, worldDesiredDirection);
            rotationNeeded.ToAngleAxis(out var angle, out var axis);

            // Convert world rotation axis to local space for input
            var localAxis = transform.InverseTransformDirection(axis);

            // Normalize rotation input (-1 to 1 range)
            var rotScale = Mathf.Clamp01(angle / 45f); // Full rotation input within 45 degrees
            var rotationInput = new Vector3(
                Mathf.Clamp(localAxis.x * rotScale, -1f, 1f),
                Mathf.Clamp(localAxis.y * rotScale, -1f, 1f),
                0f // No roll for simplicity
            );

            // Only thrust forward when roughly facing the target
            var alignment = Vector3.Dot(currentForward, worldDesiredDirection);
            var effectiveThrust = alignment > 0.5f ? thrustFraction : 0f;

            return ShipMovementInput.FromAxes(
                new Vector3(effectiveThrust, 0f, 0f), // Forward thrust only
                rotationInput,
                Mathf.Approximately(thrustFraction, 0f) // Brake when no thrust desired
            );
        }

        void ApplyBrake()
        {
            var input = ShipMovementInput.FromAxes(Vector3.zero, Vector3.zero, true);
            _movement.SimulateInput(input, Time.deltaTime);
            transform.SetPositionAndRotation(_movement.Model.Position, _movement.Model.Rotation);
        }
    }
}
