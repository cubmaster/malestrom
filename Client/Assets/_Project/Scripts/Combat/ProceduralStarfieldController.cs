using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    public sealed class ProceduralStarfieldController : MonoBehaviour
    {
        [SerializeField] float _mouseParallaxStrength = 0.004f;
        [SerializeField] float _maxParallaxRadians = 0.45f;
        [SerializeField] float _parallaxDecaySpeed = 6f;

        Vector2 _parallaxEuler;

        void LateUpdate()
        {
            if (ProceduralStarfieldEnvironment.ActiveSkyboxMaterial == null)
            {
                return;
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                var delta = mouse.delta.ReadValue();
                if (delta.sqrMagnitude > 0.01f)
                {
                    _parallaxEuler.x = Mathf.Clamp(
                        _parallaxEuler.x - delta.x * _mouseParallaxStrength,
                        -_maxParallaxRadians,
                        _maxParallaxRadians);
                    _parallaxEuler.y = Mathf.Clamp(
                        _parallaxEuler.y + delta.y * _mouseParallaxStrength,
                        -_maxParallaxRadians,
                        _maxParallaxRadians);
                }
            }

            _parallaxEuler = Vector2.Lerp(_parallaxEuler, Vector2.zero, Time.deltaTime * _parallaxDecaySpeed);
            ProceduralStarfieldEnvironment.SetParallaxEuler(_parallaxEuler);
        }
    }
}
