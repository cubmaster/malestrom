using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Third-person chase camera with smooth follow and collision pull-in.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ChaseCameraRig : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Vector3 _localOffset = new Vector3(0f, 4f, -12f);
        [SerializeField] float _positionLerp = 8f;
        [SerializeField] float _rotationLerp = 8f;
        [SerializeField] float _collisionRadius = 0.35f;
        [SerializeField] float _collisionPadding = 0.25f;
        [SerializeField] float _minArmDistance = 2f;
        [SerializeField] LayerMask _collisionLayers = Physics.DefaultRaycastLayers;

        public void SetTarget(Transform target) => _target = target;

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var pivot = _target.position;
            var desiredEye = _target.TransformPoint(_localOffset);
            var hasObstruction = ChaseCameraPlacement.TrySphereCast(
                pivot,
                desiredEye,
                _collisionRadius,
                _collisionLayers,
                out var hit);

            var obstructionDistance = hasObstruction ? hit.distance : float.PositiveInfinity;
            var resolvedEye = ChaseCameraPlacement.ResolveEyePosition(
                pivot,
                desiredEye,
                obstructionDistance,
                hasObstruction,
                _collisionPadding,
                _minArmDistance);

            var positionT = 1f - Mathf.Exp(-_positionLerp * Time.deltaTime);
            var rotationT = 1f - Mathf.Exp(-_rotationLerp * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, resolvedEye, positionT);
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, rotationT);
        }
    }
}
