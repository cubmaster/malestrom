using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ShipFlightTelemetryAdapter))]
    public sealed class ShipWeaponsInputController : MonoBehaviour
    {
        ShipFlightTelemetryAdapter _telemetry;

        void Awake()
        {
            _telemetry = GetComponent<ShipFlightTelemetryAdapter>();
        }

        void Update()
        {
            if (_telemetry == null || !enabled)
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            // Key 1 -> Slot 0 (Rail 1)
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                _telemetry.FireWeaponSlot(0);
            }

            // Key 2 -> Slot 1 (Rail 2)
            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                _telemetry.FireWeaponSlot(1);
            }

            // Key 3 -> Slot 2 (Beam 1)
            _telemetry.SetBeamFiringActive(keyboard.digit3Key.isPressed);

            // Key 4 -> Slot 3 (MSLA)
            if (keyboard.digit4Key.wasPressedThisFrame)
            {
                _telemetry.FireWeaponSlot(3);
            }

            // Key 5 -> Slot 4 (MSLB)
            if (keyboard.digit5Key.wasPressedThisFrame)
            {
                _telemetry.FireWeaponSlot(4);
            }
        }
    }
}