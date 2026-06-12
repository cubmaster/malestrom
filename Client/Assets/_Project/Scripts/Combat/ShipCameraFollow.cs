using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// Third-person chase camera for the player ship.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ShipCameraFollow : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Vector3 _localOffset = new Vector3(0f, 4f, -12f);
        [SerializeField] float _followLerp = 8f;

        public void SetTarget(Transform target) => _target = target;

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            var desired = _target.TransformPoint(_localOffset);
            transform.position = Vector3.Lerp(transform.position, desired, _followLerp * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, _followLerp * Time.deltaTime);
        }
    }
}
