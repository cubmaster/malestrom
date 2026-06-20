using IronExiles.Combat;
using UnityEngine;

namespace IronExiles.UI
{
    /// <summary>
    /// Binds the flight HUD to the player ship telemetry after scene spawn.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FlightHudBootstrap : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(BindWhenPlayerReady());
        }

        System.Collections.IEnumerator BindWhenPlayerReady()
        {
            for (var i = 0; i < 600; i++)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null && player.TryGetComponent<IShipFlightTelemetry>(out var telemetry))
                {
                    var hud = gameObject.GetComponent<FlightHudController>();
                    if (hud == null)
                    {
                        hud = gameObject.AddComponent<FlightHudController>();
                    }

                    hud.Bind(telemetry);
                    yield break;
                }

                yield return null;
            }
        }
    }
}
