using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipBeamWeaponController))]
    [RequireComponent(typeof(NetworkShipTargetingController))]
    public sealed class BeamWeaponVfx : MonoBehaviour
    {
        [SerializeField] Color _beamColor = new(0.3f, 0.85f, 1f, 0.9f);
        [SerializeField] float _beamWidth = 0.15f;

        LineRenderer _lineRenderer;
        NetworkShipBeamWeaponController _beamWeapon;
        NetworkShipTargetingController _targeting;

        void Awake()
        {
            _beamWeapon = GetComponent<NetworkShipBeamWeaponController>();
            _targeting = GetComponent<NetworkShipTargetingController>();
            _lineRenderer = gameObject.GetComponent<LineRenderer>();
            if (_lineRenderer == null)
            {
                _lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = _beamWidth;
            _lineRenderer.endWidth = _beamWidth;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = _beamColor;
            _lineRenderer.endColor = _beamColor;
            _lineRenderer.enabled = false;
        }

        void LateUpdate()
        {
            if (_lineRenderer == null || _beamWeapon == null || _targeting == null)
            {
                return;
            }

            var lockedTarget = _targeting.GetLockedTarget();
            var showBeam = _beamWeapon.IsFiring && lockedTarget != null;
            _lineRenderer.enabled = showBeam;
            if (!showBeam)
            {
                return;
            }

            var origin = transform.position;
            var end = lockedTarget.transform.position;
            _lineRenderer.SetPosition(0, origin);
            _lineRenderer.SetPosition(1, end);
        }
    }
}
