using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Pure camera placement math (testable without Play Mode physics).
    /// </summary>
    public static class ChaseCameraPlacement
    {
        public static Vector3 ResolveEyePosition(
            Vector3 pivot,
            Vector3 desiredEye,
            float obstructionDistance,
            bool hasObstruction,
            float collisionPadding,
            float minDistanceFromPivot)
        {
            if (!hasObstruction)
            {
                return desiredEye;
            }

            var offset = desiredEye - pivot;
            var magnitude = offset.magnitude;
            if (magnitude <= Mathf.Epsilon)
            {
                return desiredEye;
            }

            var direction = offset / magnitude;
            var resolvedDistance = Mathf.Max(
                minDistanceFromPivot,
                obstructionDistance - collisionPadding);

            return pivot + direction * resolvedDistance;
        }

        public static bool TrySphereCast(
            Vector3 pivot,
            Vector3 desiredEye,
            float radius,
            int layerMask,
            out RaycastHit hit)
        {
            var direction = desiredEye - pivot;
            var distance = direction.magnitude;
            if (distance <= Mathf.Epsilon)
            {
                hit = default;
                return false;
            }

            return Physics.SphereCast(
                pivot,
                radius,
                direction.normalized,
                out hit,
                distance,
                layerMask,
                QueryTriggerInteraction.Ignore);
        }
    }
}
