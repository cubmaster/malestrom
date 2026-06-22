using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    /// <summary>
    /// Keyboard thrust + mouse pitch/yaw + keyboard roll (Input System, no action assets).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipInputController : MonoBehaviour
    {
        const float MouseSensitivity = 0.15f;

        Vector3 _thrustInput;
        Vector3 _rotationInput;

        public ShipMovementInput CaptureInput()
        {
            GatherInput();
            return ShipMovementInput.FromAxes(_thrustInput, _rotationInput);
        }

        public void ReadInto(ShipMovementModel model)
        {
            GatherInput();
            model.SetMovementInput(_thrustInput, _rotationInput);
        }

        void GatherInput()
        {
            _thrustInput = Vector3.zero;
            _rotationInput = Vector3.zero;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed)
                {
                    _thrustInput.x += 1f;
                }

                if (keyboard.sKey.isPressed)
                {
                    _thrustInput.x -= 1f;
                }

                if (keyboard.dKey.isPressed)
                {
                    _thrustInput.y += 1f;
                }

                if (keyboard.aKey.isPressed)
                {
                    _thrustInput.y -= 1f;
                }

                if (keyboard.spaceKey.isPressed)
                {
                    _thrustInput.z += 1f;
                }

                if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
                {
                    _thrustInput.z -= 1f;
                }

                if (keyboard.qKey.isPressed)
                {
                    _rotationInput.z -= 1f;
                }

                if (keyboard.eKey.isPressed)
                {
                    _rotationInput.z += 1f;
                }
            }

            var mouse = Mouse.current;
            if (mouse != null)
            {
                var delta = mouse.delta.ReadValue() * MouseSensitivity;
                _rotationInput.y += delta.x;
                _rotationInput.x -= delta.y;
            }
        }
    }
}
