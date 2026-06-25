using UnityEngine;
using UnityEngine.InputSystem;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkShipTargetingController))]
    public sealed class ShipTargetInputController : MonoBehaviour
    {
        NetworkShipTargetingController _targeting;

        void Awake()
        {
            _targeting = GetComponent<NetworkShipTargetingController>();
        }

        void Update()
        {
            if (_targeting == null || (_targeting.IsSpawned && !_targeting.IsOwner))
            {
                return;
            }

            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.tabKey.wasPressedThisFrame)
            {
                if (_targeting.IsSpawned)
                {
                    _targeting.CycleTargetServerRpc(1);
                }
                else
                {
                    _targeting.CycleTargetOffline(1);
                }
            }
        }
    }
}
