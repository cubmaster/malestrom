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
            float bestCosAngle = -1f; // We want largest cosine (smallest angle)
            float maxAngleDegrees = 15f; // Lock-on cone of 15 degrees from crosshair
            float minCosAngle = Mathf.Cos(maxAngleDegrees * Mathf.Deg2Rad);

            var selfId = GetComponent<TargetableEntity>()?.GetNetworkObjectId() ?? 0UL;

            foreach (var entity in Object.FindObjectsByType<TargetableEntity>(FindObjectsSortMode.None))
            {
                if (entity == null) continue;
                var entityId = entity.GetNetworkObjectId();
                if (entityId == 0UL || entityId == selfId) continue;

                // Check distance
                var distance = Vector3.Distance(transform.position, entity.transform.position);
                if (distance > _targeting.LockRangeMeters) continue;

                // Check direction
                var toTarget = (entity.transform.position - ray.origin).normalized;
                var cosAngle = Vector3.Dot(ray.direction, toTarget);

                if (cosAngle > minCosAngle && cosAngle > bestCosAngle)
                {
                    bestCosAngle = cosAngle;
                    bestTarget = entity;
                }
            }

            if (bestTarget != null)
            {
                var targetId = bestTarget.GetNetworkObjectId();
                if (_targeting.IsSpawned)
                {
                    _targeting.RequestLockServerRpc(targetId);
                }
                else
                {
                    _targeting.RequestLockOffline(targetId);
                }
                Debug.Log($"[Targeting] Locked onto {bestTarget.DisplayName} in crosshair.");
            }
        }
    }
}
