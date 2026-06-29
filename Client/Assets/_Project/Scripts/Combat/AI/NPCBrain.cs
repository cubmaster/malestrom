using System;
using UnityEngine;

namespace IronExiles.Combat.AI
{
    /// <summary>
    /// Server-side NPC AI state machine. Drives patrol, aggro detection, combat engagement.
    /// Only active on the server; clients receive replicated position/state via NetworkObject.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NPCBrain : MonoBehaviour
    {
        NPCBrainState _state = NPCBrainState.Idle;
        Vector3 _spawnOrigin;
        Vector3 _currentWaypoint;
        float _waypointTimer;
        float _losBlockedTimer;
        Transform _currentTarget;
        NetworkShipTargetingController _targeting;
        NetworkShipBeamWeaponController _beamWeapon;
        NetworkDamageableHealth _health;
        bool _isServerActive;

        public NPCBrainState CurrentState => _state;
        public Transform CurrentTarget => _currentTarget;
        public Vector3 SpawnOrigin => _spawnOrigin;
        public Vector3 CurrentWaypoint => _currentWaypoint;

        /// <summary>
        /// Fired when the brain transitions to a new state.
        /// </summary>
        public event Action<NPCBrainState, NPCBrainState> StateChanged;

        public void Initialize(Vector3 spawnOrigin, bool isServer)
        {
            _spawnOrigin = spawnOrigin;
            _isServerActive = isServer;
            _targeting = GetComponent<NetworkShipTargetingController>();
            _beamWeapon = GetComponent<NetworkShipBeamWeaponController>();
            _health = GetComponent<NetworkDamageableHealth>();

            if (_health != null)
            {
                _health.Destroyed += OnDestroyed;
            }

            if (_isServerActive)
            {
                TransitionTo(NPCBrainState.Patrol);
                PickNewWaypoint();
            }
        }

        void Update()
        {
            if (!_isServerActive || _state == NPCBrainState.Dead)
            {
                return;
            }

            switch (_state)
            {
                case NPCBrainState.Patrol:
                    UpdatePatrol();
                    break;
                case NPCBrainState.Combat:
                    UpdateCombat();
                    break;
            }
        }

        void UpdatePatrol()
        {
            _waypointTimer += Time.deltaTime;
            if (_waypointTimer >= NPCSettings.PatrolWaypointInterval)
            {
                PickNewWaypoint();
            }

            var nearestPlayer = FindNearestPlayer();
            if (nearestPlayer != null)
            {
                var distance = Vector3.Distance(transform.position, nearestPlayer.position);
                if (distance <= NPCSettings.AggroRadius)
                {
                    _currentTarget = nearestPlayer;
                    TransitionTo(NPCBrainState.Combat);
                    SetWeaponFiring(true);
                    TryLockTarget(nearestPlayer);
                }
            }
        }

        void UpdateCombat()
        {
            if (_currentTarget == null || !_currentTarget.gameObject.activeInHierarchy)
            {
                Disengage();
                return;
            }

            var distance = Vector3.Distance(transform.position, _currentTarget.position);

            if (distance > NPCSettings.DisengageRadius)
            {
                Disengage();
                return;
            }

            // Check LOS
            if (!HasLineOfSight(_currentTarget))
            {
                _losBlockedTimer += Time.deltaTime;
                if (_losBlockedTimer >= NPCSettings.LosBreakSeconds)
                {
                    Disengage();
                    return;
                }
            }
            else
            {
                _losBlockedTimer = 0f;
            }

            // Check if target is dead
            var targetHealth = _currentTarget.GetComponent<NetworkDamageableHealth>();
            if (targetHealth != null && targetHealth.IsDestroyed)
            {
                Disengage();
                return;
            }
        }

        void Disengage()
        {
            _currentTarget = null;
            _losBlockedTimer = 0f;
            SetWeaponFiring(false);
            TransitionTo(NPCBrainState.Patrol);
            PickNewWaypoint();
        }

        void OnDestroyed(ulong networkObjectId)
        {
            TransitionTo(NPCBrainState.Dead);
            SetWeaponFiring(false);
            _currentTarget = null;
        }

        void TransitionTo(NPCBrainState newState)
        {
            if (_state == newState)
            {
                return;
            }

            var oldState = _state;
            _state = newState;
            StateChanged?.Invoke(oldState, newState);
        }

        void PickNewWaypoint()
        {
            _waypointTimer = 0f;
            var randomOffset = UnityEngine.Random.insideUnitSphere * NPCSettings.PatrolRadius;
            randomOffset.y *= 0.3f; // Bias patrol to be more horizontal
            _currentWaypoint = _spawnOrigin + randomOffset;
        }

        Transform FindNearestPlayer()
        {
            Transform nearest = null;
            var nearestDist = float.MaxValue;

            foreach (var targetable in Object.FindObjectsByType<TargetableEntity>(FindObjectsSortMode.None))
            {
                if (targetable.Affiliation != TargetAffiliation.Friendly)
                {
                    continue;
                }

                if (targetable.transform == transform)
                {
                    continue;
                }

                // Friendly affiliation indicates a player ship
                var dist = Vector3.Distance(transform.position, targetable.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = targetable.transform;
                }
            }

            return nearest;
        }

        bool HasLineOfSight(Transform target)
        {
            var direction = target.position - transform.position;
            var distance = direction.magnitude;
            if (distance <= 0.01f)
            {
                return true;
            }

            if (!Physics.Raycast(transform.position, direction.normalized, out var hit, distance,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            return hit.collider != null && hit.collider.transform.IsChildOf(target);
        }

        void SetWeaponFiring(bool firing)
        {
            if (_beamWeapon == null)
            {
                return;
            }

            // Use server method for spawned networked objects, offline for tests
            _beamWeapon.SetFiringServer(firing);
        }

        void TryLockTarget(Transform target)
        {
            if (_targeting == null)
            {
                return;
            }

            var targetable = target.GetComponent<TargetableEntity>();
            if (targetable == null)
            {
                return;
            }

            var targetId = targetable.GetNetworkObjectId();
            if (targetId != 0UL)
            {
                _targeting.RequestLockServer(targetId);
            }
        }

        void OnDestroy()
        {
            if (_health != null)
            {
                _health.Destroyed -= OnDestroyed;
            }
        }
    }
}
