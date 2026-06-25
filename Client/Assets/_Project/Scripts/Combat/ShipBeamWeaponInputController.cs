using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipBeamWeaponController))]
    public sealed class ShipBeamWeaponInputController : MonoBehaviour
    {
        NetworkShipBeamWeaponController _beamWeapon;
        bool _lastSentFiring;

        void Awake()
        {
            _beamWeapon = GetComponent<NetworkShipBeamWeaponController>();
        }

        void Update()
        {
            if (_beamWeapon == null || (_beamWeapon.IsSpawned && !_beamWeapon.IsOwner))
            {
                return;
            }

            var mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            var wantsFire = mouse.leftButton.isPressed;
            if (wantsFire == _lastSentFiring)
            {
                return;
            }

            _lastSentFiring = wantsFire;

            if (_beamWeapon.IsSpawned)
            {
                _beamWeapon.SetFiringServerRpc(wantsFire);
            }
            else
            {
                _beamWeapon.SetFiringOffline(wantsFire);
            }
        }
    }
}
