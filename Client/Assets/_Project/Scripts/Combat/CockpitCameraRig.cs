using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class CockpitCameraRig : MonoBehaviour
    {
        [SerializeField] Transform _target;
        [SerializeField] Vector3 _localOffset = new Vector3(0f, 0.3f, 0.8f);
        [SerializeField] float _fieldOfView = 75f;
        [SerializeField] float _nearClip = 0.1f;
        [SerializeField] float _farClip = 10000f;

        Camera _camera;

        public void SetTarget(Transform target)
        {
            _target = target;
            ApplyCameraSettings();
        }

        void Awake()
        {
            _camera = GetComponent<Camera>();
            ApplyCameraSettings();
        }

        void ApplyCameraSettings()
        {
            if (_camera == null)
            {
                return;
            }

            _camera.fieldOfView = _fieldOfView;
            _camera.nearClipPlane = _nearClip;
            _camera.farClipPlane = _farClip;
        }

        void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            transform.position = _target.TransformPoint(_localOffset);
            transform.rotation = _target.rotation;
        }
    }
}
