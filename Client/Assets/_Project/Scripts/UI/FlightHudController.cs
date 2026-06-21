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
                _view.WirePowerSliderCallbacks(OnPowerSliderChanged, OnPowerPresetClicked);
            }

            BindPowerControl(ResolvePowerControl(telemetry));
        }

        void BindPowerControl(IShipReactorPowerControl control)
        {
            _view?.BindPowerControl(control);
        }

        static IShipReactorPowerControl ResolvePowerControl(IShipFlightTelemetry telemetry)
        {
            if (telemetry is Component component)
            {
                var network = component.GetComponent<NetworkShipReactorPowerController>();
                if (network != null)
                {
                    return network;
                }

                return component.GetComponent<ShipReactorPowerController>();
            }

            return null;
        }

        void OnPowerSliderChanged(int channelIndex) => _view?.OnPowerSliderChanged(channelIndex);

        void OnPowerPresetClicked(PowerAllocation preset) => _view?.OnPowerPresetClicked(preset);

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
