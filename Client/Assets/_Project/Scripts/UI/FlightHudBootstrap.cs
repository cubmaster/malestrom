using IronExiles.Combat;
using UnityEngine;

namespace IronExiles.UI
{
    [DisallowMultipleComponent]
    public sealed class FlightHudBootstrap : MonoBehaviour
    {
        CockpitFrameView _cockpitFrame;

        void Start()
        {
            _cockpitFrame = CockpitFrameView.Create(transform);

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

        void OnDestroy()
        {
            if (_cockpitFrame?.Canvas != null)
            {
                Destroy(_cockpitFrame.Canvas.gameObject);
            }
        }
    }
}
