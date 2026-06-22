using UnityEngine;

namespace IronExiles.Combat
{
    /// <summary>
    /// First-person cockpit camera: parents the main camera to the ship so view = ship orientation.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CockpitCameraRig : MonoBehaviour
    {
        [SerializeField] Vector3 _localEyeOffset = new Vector3(0f, 0.08f, 0f);
        [SerializeField] float _fieldOfView = 75f;
        [SerializeField] float _nearClip = 0.05f;
        [SerializeField] float _farClip = 10000f;

        Transform _ship;
        Transform _previousParent;
        Camera _camera;

        public void SetTarget(Transform ship)
        {
            if (_ship == ship)
            {
                return;
            }

            DetachFromShip();
            _ship = ship;

            if (_ship == null)
            {
                return;
            }

            _previousParent = transform.parent;
            transform.SetParent(_ship, worldPositionStays: false);
            transform.localPosition = _localEyeOffset;
            transform.localRotation = Quaternion.identity;
            ApplyCameraSettings();
        }

        public static void AttachMainCameraToShip(Transform ship)
        {
            if (ship == null)
            {
                return;
            }

            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var rig = camera.GetComponent<CockpitCameraRig>();
            if (rig == null)
            {
                rig = camera.gameObject.AddComponent<CockpitCameraRig>();
            }

            rig.SetTarget(ship);
        }

        public static void HideLocalHull(GameObject ship)
        {
            if (ship == null)
            {
                return;
            }

            var renderer = ship.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        void Awake()
        {
            _camera = GetComponent<Camera>();
            ApplyCameraSettings();
        }

        void OnDisable()
        {
            DetachFromShip();
        }

        void LateUpdate()
        {
            if (_ship == null)
            {
                return;
            }

            transform.localPosition = _localEyeOffset;
            transform.localRotation = Quaternion.identity;
        }

        void DetachFromShip()
        {
            if (_ship != null && transform.parent == _ship)
            {
                transform.SetParent(_previousParent, worldPositionStays: true);
            }

            _ship = null;
            _previousParent = null;
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
    }
}
