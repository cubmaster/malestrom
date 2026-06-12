using IronExiles.Combat;

namespace IronExiles.UI
{
    public readonly struct FlightHudDisplayState
    {
        public readonly string SpeedText;
        public readonly string HeadingText;
        public readonly float HullFill01;

        public FlightHudDisplayState(string speedText, string headingText, float hullFill01)
        {
            SpeedText = speedText;
            HeadingText = headingText;
            HullFill01 = hullFill01;
        }
    }

    public static class FlightHudPresenter
    {
        public static FlightHudDisplayState Build(IShipFlightTelemetry telemetry)
        {
            if (telemetry == null || !telemetry.IsActive)
            {
                return new FlightHudDisplayState(string.Empty, string.Empty, 0f);
            }

            var speed = telemetry.SpeedMetersPerSecond;
            var heading = telemetry.HeadingDegrees;
            var hullFill = telemetry.HullPercent / 100f;

            return new FlightHudDisplayState(
                $"{speed:F0} m/s",
                $"HDG {heading:F0}°",
                hullFill);
        }
    }
}
