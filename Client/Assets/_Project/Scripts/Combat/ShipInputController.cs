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
        bool _brakeInput;

        public ShipMovementInput CaptureInput()
        {
            GatherInput();
            return ShipMovementInput.FromAxes(_thrustInput, _rotationInput, _brakeInput);
        }

        public void ReadInto(ShipMovementModel model)
        {
            GatherInput();
            model.SetMovementInput(_thrustInput, _rotationInput, _brakeInput);
        }

        void GatherInput()
        {
            _thrustInput = Vector3.zero;
            _rotationInput = Vector3.zero;
            _brakeInput = false;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                // W/S: forward/reverse thrust
                if (keyboard.wKey.isPressed)
                    _thrustInput.x += 1f;
                if (keyboard.sKey.isPressed)
                    _thrustInput.x -= 1f;

                // A/D: yaw left/right (turning)
                if (keyboard.aKey.isPressed)
                    _rotationInput.y -= 1f;
                if (keyboard.dKey.isPressed)
                    _rotationInput.y += 1f;

                // Q/E: roll
                if (keyboard.qKey.isPressed)
                    _rotationInput.z -= 1f;
                if (keyboard.eKey.isPressed)
                    _rotationInput.z += 1f;

                // Space/Ctrl: vertical strafe (up/down)
                if (keyboard.spaceKey.isPressed)
                    _thrustInput.z += 1f;
                if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
                    _thrustInput.z -= 1f;

                // Shift: lateral strafe left/right (secondary)
                if (keyboard.leftShiftKey.isPressed)
                    _thrustInput.y -= 1f;
                if (keyboard.rightShiftKey.isPressed)
                    _thrustInput.y += 1f;

                // X: Space brake
                if (keyboard.xKey.isPressed)
                    _brakeInput = true;
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
