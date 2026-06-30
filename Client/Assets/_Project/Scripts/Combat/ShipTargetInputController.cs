using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipTargetingController))]
    public sealed class ShipTargetInputController : MonoBehaviour
    {
        NetworkShipTargetingController _targeting;

        void Awake()
        {
            _targeting = GetComponent<NetworkShipTargetingController>();
        }

        void Update()
        {
            if (_targeting == null || (_targeting.IsSpawned && !_targeting.IsOwner))
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.commaKey.wasPressedThisFrame)
            {
                _targeting.AdjustRadarRange(-500f);
            }

            if (keyboard.periodKey.wasPressedThisFrame)
            {
                _targeting.AdjustRadarRange(500f);
            }

            if (keyboard.tabKey.wasPressedThisFrame || keyboard.tKey.wasPressedThisFrame)
            {
                LockTargetUnderCrosshair();
            }
        }

        void LockTargetUnderCrosshair()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            TargetableEntity bestTarget = null;
            float bestCosAngle = -1f;
            float maxAngleDegrees = 30f;
            float minCosAngle = Mathf.Cos(maxAngleDegrees * Mathf.Deg2Rad);

            TargetableEntity nearestTarget = null;
            float nearestDist = float.MaxValue;

            var selfId = GetComponent<TargetableEntity>()?.GetNetworkObjectId() ?? 0UL;

            foreach (var entity in Object.FindObjectsByType<TargetableEntity>(FindObjectsSortMode.None))
            {
                if (entity == null) continue;
                var entityId = entity.GetNetworkObjectId();
                if (entityId == 0UL || entityId == selfId) continue;
                if (!TargetSelectionMath.IsTabSelectable(entity.Affiliation)) continue;

                var distance = Vector3.Distance(transform.position, entity.transform.position);
                if (distance > _targeting.LockRangeMeters) continue;

                // Track nearest as fallback
                if (distance < nearestDist)
                {
                    nearestDist = distance;
                    nearestTarget = entity;
                }

                // Check if in crosshair cone
                var toTarget = (entity.transform.position - ray.origin).normalized;
                var cosAngle = Vector3.Dot(ray.direction, toTarget);

                if (cosAngle > minCosAngle && cosAngle > bestCosAngle)
                {
                    bestCosAngle = cosAngle;
                    bestTarget = entity;
                }
            }

            // Use crosshair target if found, otherwise nearest in range
            var finalTarget = bestTarget ?? nearestTarget;
            if (finalTarget != null)
            {
                var targetId = finalTarget.GetNetworkObjectId();
                if (_targeting.IsSpawned)
                {
                    _targeting.RequestLockServerRpc(targetId);
                }
                else
                {
                    _targeting.RequestLockOffline(targetId);
                }
            }
        }
    }
}
