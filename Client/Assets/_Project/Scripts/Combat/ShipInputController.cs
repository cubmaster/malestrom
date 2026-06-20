using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    /// <summary>
    /// Keyboard + mouse flight input (Input System package, no action assets required).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipMovementController))]
    public sealed class ShipInputController : MonoBehaviour
    {
        const float MouseSensitivity = 0.15f;

        Vector3 _thrustInput;
        Vector3 _rotationInput;
        bool _brakeHeld;

        public ShipMovementInput CaptureInput()
        {
            GatherInput();
            return ShipMovementInput.FromAxes(_thrustInput, _rotationInput, _brakeHeld);
        }

        public void ReadInto(ShipMovementModel model)
        {
            GatherInput();
            model.SetMovementInput(_thrustInput, _rotationInput, _brakeHeld);
        }

        void GatherInput()
        {
            _thrustInput = Vector3.zero;
            _rotationInput = Vector3.zero;
            _brakeHeld = false;

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

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

            _brakeHeld = keyboard.leftShiftKey.isPressed;

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
