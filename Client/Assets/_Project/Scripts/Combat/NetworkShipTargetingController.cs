using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TargetableEntity))]
    public sealed class NetworkShipTargetingController : NetworkBehaviour
    {
        [SerializeField] float _lockRangeMeters = TargetingSensorSettings.DefaultLockRangeMeters;
        [SerializeField] int _maxRadarContacts = TargetingSensorSettings.DefaultMaxRadarContacts;
        [SerializeField] float _losBreakSeconds = TargetingSensorSettings.DefaultLosBreakSeconds;
        [SerializeField] LayerMask _lineOfSightMask = Physics.DefaultRaycastLayers;

        readonly NetworkVariable<ulong> _lockedTargetId = new(
            0UL,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        TargetableEntity _selfTargetable;
        float _losBlockedTimer;
        ulong _localLockedTargetId;

        public ulong LockedTargetNetworkObjectId => IsSpawned ? _lockedTargetId.Value : _localLockedTargetId;
        public float LockRangeMeters => _lockRangeMeters;
        public int MaxRadarContacts => _maxRadarContacts;
        public bool ProvidesLocalRadar => !IsSpawned || IsOwner;

        void Awake()
        {
            _selfTargetable = GetComponent<TargetableEntity>();
        }

        private void SetLockedTargetId(ulong value)
        {
            if (IsSpawned)
            {
                _lockedTargetId.Value = value;
            }
            else
            {
                _localLockedTargetId = value;
            }
        }

        public TargetableEntity GetLockedTarget()
        {
            var targetId = LockedTargetNetworkObjectId;
            if (targetId == 0UL)
            {
                return null;
            }

            return TargetSelectionMath.TryResolveTargetable(targetId, FindAllTargetables(), out var target)
                ? target
                : null;
        }

        public IReadOnlyList<RadarContact> GetRadarContacts()
        {
            if (IsSpawned && !IsOwner)
            {
                return System.Array.Empty<RadarContact>();
            }

            return TargetSelectionMath.CollectRadarContacts(
                transform.position,
                transform.forward,
                _selfTargetable.GetNetworkObjectId(),
                FindAllTargetables(),
                _lockRangeMeters,
                _maxRadarContacts);
        }

        [ServerRpc]
        public void CycleTargetServerRpc(int direction, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            var candidates = TargetSelectionMath.CollectTabCandidates(
                transform.position,
                transform.forward,
                _selfTargetable.GetNetworkObjectId(),
                FindAllTargetables(),
                _lockRangeMeters);

            if (candidates.Count == 0)
            {
                ClearLock();
                return;
            }

            var nextIndex = TargetSelectionMath.SelectNextTabIndex(candidates, LockedTargetNetworkObjectId, direction);
            if (nextIndex < 0 || nextIndex >= candidates.Count)
            {
                return;
            }

            TrySetLock(candidates[nextIndex].NetworkObjectId);
        }

        public void CycleTargetOffline(int direction)
        {
            if (IsSpawned)
            {
                return;
            }

            var selfId = _selfTargetable != null ? _selfTargetable.GetNetworkObjectId() : 0UL;
            var candidates = TargetSelectionMath.CollectTabCandidates(
                transform.position,
                transform.forward,
                selfId,
                FindAllTargetables(),
                _lockRangeMeters);

            if (candidates.Count == 0)
            {
                ClearLock();
                return;
            }

            var nextIndex = TargetSelectionMath.SelectNextTabIndex(candidates, LockedTargetNetworkObjectId, direction);
            if (nextIndex < 0 || nextIndex >= candidates.Count)
            {
                return;
            }

            TrySetLock(candidates[nextIndex].NetworkObjectId);
        }

        [ServerRpc]
        public void RequestLockServerRpc(ulong targetNetworkObjectId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            TrySetLock(targetNetworkObjectId);
        }

        void Update()
        {
            if (IsSpawned && !IsServer)
            {
                return;
            }

            ValidateCurrentLock();
        }

        void ValidateCurrentLock()
        {
            var targetId = LockedTargetNetworkObjectId;
            if (targetId == 0UL)
            {
                _losBlockedTimer = 0f;
                return;
            }

            if (!TargetSelectionMath.TryResolveTargetable(targetId, FindAllTargetables(), out var target))
            {
                ClearLock();
                return;
            }

            var targetPosition = target.transform.position;
            if (!TargetSelectionMath.IsWithinLockRange(transform.position, targetPosition, _lockRangeMeters))
            {
                ClearLock();
                return;
            }

            if (HasLineOfSight(target))
            {
                _losBlockedTimer = 0f;
                return;
            }

            _losBlockedTimer += Time.deltaTime;
            if (_losBlockedTimer >= _losBreakSeconds)
            {
                ClearLock();
            }
        }

        bool TrySetLock(ulong targetNetworkObjectId)
        {
            var selfId = _selfTargetable != null ? _selfTargetable.GetNetworkObjectId() : 0UL;
            if (targetNetworkObjectId == 0UL || targetNetworkObjectId == selfId)
            {
                return false;
            }

            if (!TargetSelectionMath.TryResolveTargetable(targetNetworkObjectId, FindAllTargetables(), out var target))
            {
                return false;
            }

            if (!TargetSelectionMath.IsTabSelectable(target.Affiliation))
            {
                return false;
            }

            if (!TargetSelectionMath.IsWithinLockRange(transform.position, target.transform.position, _lockRangeMeters))
            {
                return false;
            }

            SetLockedTargetId(targetNetworkObjectId);
            _losBlockedTimer = 0f;
            return true;
        }

        void ClearLock()
        {
            SetLockedTargetId(0UL);
            _losBlockedTimer = 0f;
        }

        bool HasLineOfSight(TargetableEntity target)
        {
            var origin = transform.position;
            var targetPosition = target.transform.position;
            var direction = targetPosition - origin;
            var distance = direction.magnitude;
            if (distance <= 0.01f)
            {
                return true;
            }

            if (!Physics.Raycast(origin, direction / distance, out var hit, distance, _lineOfSightMask, QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            return hit.collider != null && hit.collider.transform.IsChildOf(target.transform);
        }

        static IEnumerable<TargetableEntity> FindAllTargetables()
        {
            return Object.FindObjectsByType<TargetableEntity>();
        }
    }
}
