using IronExiles.Combat;
using UnityEngine;

namespace IronExiles.UI
{
    [DisallowMultipleComponent]
    public sealed class FlightHudController : MonoBehaviour
    {
        IShipFlightTelemetry _telemetry;
        FlightHudView _view;

        public void Bind(IShipFlightTelemetry telemetry)
        {
            _telemetry = telemetry;
            if (_view == null)
            {
                _view = FlightHudView.Create(transform);
            }
        }

        void LateUpdate()
        {
            if (_view == null)
            {
                return;
            }

            _view.Apply(FlightHudPresenter.Build(_telemetry));
        }

        void OnDestroy()
        {
            if (_view?.Canvas != null)
            {
                Destroy(_view.Canvas.gameObject);
            }
        }
    }
}
