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

            Debug.Log($"[NetworkedShipSetup] OnNetworkSpawn — IsOwner={IsOwner}, IsClient={IsClient}, IsServer={IsServer}, OwnerClientId={OwnerClientId}, LocalClientId={NetworkManager.Singleton?.LocalClientId}");

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
                Debug.Log("[NetworkedShipSetup] Attaching local player systems (owner).");
                AttachLocalPlayerSystems();
            }
            else
            {
                Debug.Log("[NetworkedShipSetup] Configuring as remote ship (not owner).");
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

            var damageable = GetComponent<NetworkDamageableHealth>();
            if (damageable != null && IsServer)
            {
                damageable.ConfigureForServer(BeamWeaponSettings.DefaultMaxHull);
            }

            var shieldController = GetComponent<NetworkShipShieldController>();
            if (shieldController != null && IsServer)
            {
                shieldController.ConfigureForServer(ShieldSettings.DefaultMaxShieldPerFacing);
            }
        }

        void AttachLocalPlayerSystems()
        {
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            gameObject.tag = "Player";

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            var visuals = transform.Find("Visuals");
            if (visuals != null)
            {
                visuals.gameObject.SetActive(false);
            }

            CockpitCameraRig.AttachMainCameraToShip(transform);

            if (GetComponent<ShipFlightTelemetryAdapter>() == null)
            {
                gameObject.AddComponent<ShipFlightTelemetryAdapter>();
            }

            if (GetComponent<ShipWeaponsInputController>() == null)
            {
                gameObject.AddComponent<ShipWeaponsInputController>();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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

            var visuals = transform.Find("Visuals");
            if (visuals != null)
            {
                visuals.gameObject.SetActive(true);
            }
        }
    }
}
