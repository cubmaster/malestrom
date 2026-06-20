using UnityEngine;

namespace IronExiles.Combat
{
    public enum MovementReconcileMode
    {
        None,
        Blend,
        Snap
    }

    public static class ShipMovementReplicationMath
    {
        public static MovementReconcileMode EvaluateReconcileMode(
            Vector3 serverPosition,
            Quaternion serverRotation,
            Vector3 predictedPosition,
            Quaternion predictedRotation,
            float positionThresholdMeters,
            float rotationThresholdDegrees)
        {
            var positionError = Vector3.Distance(serverPosition, predictedPosition);
            if (positionError >= positionThresholdMeters)
            {
                return MovementReconcileMode.Snap;
            }

            var rotationError = Quaternion.Angle(serverRotation, predictedRotation);
            if (rotationError >= rotationThresholdDegrees)
            {
                return MovementReconcileMode.Snap;
            }

            if (positionError > 0.05f || rotationError > 1f)
            {
                return MovementReconcileMode.Blend;
            }

            return MovementReconcileMode.None;
        }

        public static Vector3 BlendPosition(Vector3 current, Vector3 target, float deltaTime, float blendSpeed)
        {
            return Vector3.Lerp(current, target, Mathf.Clamp01(blendSpeed * deltaTime));
        }

        public static Quaternion BlendRotation(Quaternion current, Quaternion target, float deltaTime, float blendSpeed)
        {
            return Quaternion.Slerp(current, target, Mathf.Clamp01(blendSpeed * deltaTime));
        }
    }
}
