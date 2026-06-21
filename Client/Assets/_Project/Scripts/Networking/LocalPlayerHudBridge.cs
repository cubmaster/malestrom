using System.Collections;
using IronExiles.Combat;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.Networking
{
    [DisallowMultipleComponent]
    public sealed class LocalPlayerHudBridge : MonoBehaviour
    {
        void OnEnable()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnClientConnectedCallback += OnClientConnected;

            if (networkManager.IsConnectedClient)
            {
                StartCoroutine(NotifyWhenLocalPlayerReady());
            }
        }

        void OnDisable()
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null)
            {
                return;
            }

            networkManager.OnClientConnectedCallback -= OnClientConnected;
        }

        void OnClientConnected(ulong clientId)
        {
            var networkManager = NetworkManager.Singleton;
            if (networkManager == null || clientId != networkManager.LocalClientId)
            {
                return;
            }

            StartCoroutine(NotifyWhenLocalPlayerReady());
        }

        IEnumerator NotifyWhenLocalPlayerReady()
        {
            for (var i = 0; i < 600; i++)
            {
                var networkManager = NetworkManager.Singleton;
                var localPlayer = networkManager != null && networkManager.IsConnectedClient
                    ? networkManager.SpawnManager?.GetLocalPlayerObject()
                    : null;

                if (localPlayer != null)
                {
                    var playerObject = localPlayer.gameObject;
                    if (!playerObject.activeInHierarchy)
                    {
                        playerObject.SetActive(true);
                    }

                    if (!playerObject.CompareTag("Player"))
                    {
                        playerObject.tag = "Player";
                    }

                    if (playerObject.GetComponent<ShipFlightTelemetryAdapter>() == null)
                    {
                        playerObject.AddComponent<ShipFlightTelemetryAdapter>();
                    }

                    LocalPlayerSystemsEvents.NotifyLocalPlayerShipReady(playerObject);
                    Debug.Log("[LocalPlayerHudBridge] Local player ready for HUD binding.");
                    yield break;
                }

                yield return null;
            }

            Debug.LogWarning("[LocalPlayerHudBridge] Timed out waiting for local player object.");
        }
    }
}
