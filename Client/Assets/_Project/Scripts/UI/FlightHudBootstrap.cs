using IronExiles.Combat;
using Unity.Netcode;
using UnityEngine;

namespace IronExiles.UI
{
    /// <summary>
    /// Binds the flight HUD to the player ship telemetry after scene spawn.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FlightHudBootstrap : MonoBehaviour
    {
        bool _bound;
        int _failedBindLogCount;

        void OnEnable()
        {
            _bound = false;
            _failedBindLogCount = 0;
            LocalPlayerSystemsEvents.LocalPlayerShipReady += OnLocalPlayerShipReady;
        }

        void OnDisable()
        {
            LocalPlayerSystemsEvents.LocalPlayerShipReady -= OnLocalPlayerShipReady;
            _bound = false;
        }

        void Start()
        {
            Debug.Log("[FlightHudBootstrap] Waiting for local player ship...");
            StartCoroutine(BindWhenPlayerReady());
        }

        void OnLocalPlayerShipReady(GameObject player)
        {
            TryBindPlayer(player);
        }

        System.Collections.IEnumerator BindWhenPlayerReady()
        {
            while (!_bound)
            {
                TryBindPlayer(FindLocalPlayerShip());
                yield return null;
            }
        }

        static GameObject FindLocalPlayerShip()
        {
            var tagged = GameObject.FindGameObjectWithTag("Player");
            if (tagged != null)
            {
                return tagged;
            }

            var networkManager = NetworkManager.Singleton;
            if (networkManager != null && networkManager.IsConnectedClient)
            {
                var localPlayer = networkManager.SpawnManager?.GetLocalPlayerObject();
                if (localPlayer != null)
                {
                    return localPlayer.gameObject;
                }
            }

            foreach (var adapter in Object.FindObjectsByType<ShipFlightTelemetryAdapter>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (adapter.isActiveAndEnabled)
                {
                    return adapter.gameObject;
                }
            }

            return null;
        }

        void TryBindPlayer(GameObject player)
        {
            if (_bound)
            {
                return;
            }

            if (player == null)
            {
                LogBindFailure("no Player-tagged ship found yet");
                return;
            }

            var telemetry = player.GetComponent<ShipFlightTelemetryAdapter>();
            if (telemetry == null)
            {
                LogBindFailure($"ship '{player.name}' has no telemetry adapter yet");
                return;
            }

            var hud = gameObject.GetComponent<FlightHudController>();
            if (hud == null)
            {
                hud = gameObject.AddComponent<FlightHudController>();
            }

            hud.Bind(telemetry);

            var indicator = gameObject.GetComponent<DamageDirectionIndicator>();
            if (indicator == null)
            {
                indicator = gameObject.AddComponent<DamageDirectionIndicator>();
                var canvas = hud.GetComponentInChildren<Canvas>();
                if (canvas == null)
                {
                    canvas = Object.FindFirstObjectByType<Canvas>();
                }
                indicator.Initialize(canvas, player.transform);
                LocalPlayerSystemsEvents.LocalPlayerHit += indicator.ShowHit;
            }

            _bound = true;
            Debug.Log($"[FlightHudBootstrap] Bound flight HUD to local player ship '{player.name}'.");
        }

        void LogBindFailure(string reason)
        {
            if (_failedBindLogCount >= 5)
            {
                return;
            }

            _failedBindLogCount++;
            Debug.Log($"[FlightHudBootstrap] Not bound yet: {reason}.");
        }
    }
}
