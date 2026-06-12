using IronExiles.Combat;
using IronExiles.UI;
using NUnit.Framework;

namespace IronExiles.Core.Tests
{
    public class FlightHudPresenterTests
    {
        sealed class StubTelemetry : IShipFlightTelemetry
        {
            public float SpeedMetersPerSecond { get; set; }
            public float HeadingDegrees { get; set; }
            public float HullPercent { get; set; } = 100f;
            public bool IsActive { get; set; } = true;
        }

        [Test]
        public void Build_WithMovement_ReportsPositiveSpeed()
        {
            var telemetry = new StubTelemetry
            {
                SpeedMetersPerSecond = 42.7f,
                HeadingDegrees = 90f,
            };

            var state = FlightHudPresenter.Build(telemetry);

            Assert.AreEqual("43 m/s", state.SpeedText);
            Assert.AreEqual("HDG 90°", state.HeadingText);
            Assert.AreEqual(1f, state.HullFill01, 0.001f);
        }

        [Test]
        public void Build_WhenInactive_ReturnsEmptyState()
        {
            var telemetry = new StubTelemetry { IsActive = false, SpeedMetersPerSecond = 10f };

            var state = FlightHudPresenter.Build(telemetry);

            Assert.AreEqual(string.Empty, state.SpeedText);
            Assert.AreEqual(string.Empty, state.HeadingText);
            Assert.AreEqual(0f, state.HullFill01);
        }
    }
}
