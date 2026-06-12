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
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            if (!player.TryGetComponent<IShipFlightTelemetry>(out var telemetry))
            {
                return;
            }

            var hud = gameObject.GetComponent<FlightHudController>();
            if (hud == null)
            {
                hud = gameObject.AddComponent<FlightHudController>();
            }

            hud.Bind(telemetry);
        }
    }
}
