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

            ConfigureTargetableAffiliation();

            if (IsOwner)
            {
                AttachLocalPlayerSystems();
            }
            else
            {
                ConfigureRemoteShipVisual();
            }
        }

        void ConfigureTargetableAffiliation()
        {
            var targetable = GetComponent<TargetableEntity>();
            if (targetable == null)
            {
                return;
            }

            if (IsOwner)
            {
                targetable.Configure("Player Ship", TargetAffiliation.Friendly, 100f);
            }
            else
            {
                targetable.Configure($"Hostile {OwnerClientId}", TargetAffiliation.Hostile, 100f);
            }
        }

        void AttachLocalPlayerSystems()
        {
            gameObject.tag = "Player";
            CockpitCameraRig.HideLocalHull(gameObject);
            CockpitCameraRig.AttachMainCameraToShip(transform);

            gameObject.AddComponent<ShipFlightTelemetryAdapter>();
            LocalPlayerSystemsEvents.NotifyLocalPlayerShipReady(gameObject);
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
