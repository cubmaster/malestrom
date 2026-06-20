using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkObject))]
    public sealed class NetworkedShipSetup : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            var inputController = GetComponent<ShipInputController>();
            if (inputController != null)
            {
                inputController.enabled = IsOwner;
            }

            var movementController = GetComponent<ShipMovementController>();
            if (movementController != null && GetComponent<NetworkShipMovementController>() == null)
            {
                movementController.enabled = IsOwner;
            }

            if (IsOwner)
            {
                AttachLocalPlayerSystems();
            }
            else
            {
                ConfigureRemoteShipVisual();
            }
        }

        void AttachLocalPlayerSystems()
        {
            gameObject.tag = "Player";

            var camera = Camera.main;
            if (camera != null)
            {
                var rig = camera.gameObject.GetComponent<CockpitCameraRig>();
                if (rig == null)
                {
                    rig = camera.gameObject.AddComponent<CockpitCameraRig>();
                }

                rig.SetTarget(transform);
            }

            gameObject.AddComponent<ShipFlightTelemetryAdapter>();
        }

        void ConfigureRemoteShipVisual()
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = true;
                renderer.material.color = new Color(0.6f, 0.6f, 0.7f, 1f);
            }
        }
    }
}
