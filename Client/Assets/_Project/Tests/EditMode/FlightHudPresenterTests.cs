using IronExiles.Combat;
using IronExiles.UI;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public class FlightHudPresenterTests
    {
        sealed class StubTelemetry : IShipFlightTelemetry
        {
            public float SpeedMetersPerSecond { get; set; }
            public float HeadingDegrees { get; set; }
            public float HullPercent { get; set; } = 100f;
            public float ShieldPercent { get; set; } = 100f;
            public float JumpDriveChargePercent { get; set; } = 100f;
            public bool JumpDriveReady { get; set; } = true;
            public PowerAllocation Power { get; set; } = new PowerAllocation
            {
                Weapons = 0.5f, Shields = 0.3f, Engines = 0.12f, Ecm = 0.08f
            };
            public int HardpointCount => 0;
            public HardpointStatus GetHardpoint(int index) => default;
            public int RadarContactCount => 0;
            public Vector3 GetRadarContact(int index) => Vector3.zero;
            public string LockedTargetName => string.Empty;
            public float LockedTargetDistanceMeters => 0f;
            public float LockedTargetHullFill01 => 0f;
            public ulong LockedTargetNetworkObjectId => 0UL;
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

            Assert.IsTrue(state.IsVisible);
            Assert.AreEqual("43 m/s", state.SpeedText);
            Assert.AreEqual("HDG 90°", state.HeadingText);
            Assert.AreEqual(1f, state.HullFill01, 0.001f);
        }

        [Test]
        public void Build_WhenInactive_StillShowsHud()
        {
            var telemetry = new StubTelemetry { IsActive = false, SpeedMetersPerSecond = 10f };

            var state = FlightHudPresenter.Build(telemetry);

            Assert.IsTrue(state.IsVisible);
            Assert.AreEqual("10 m/s", state.SpeedText);
        }

        [Test]
        public void Build_ReportsJumpDriveStatus()
        {
            var telemetry = new StubTelemetry
            {
                SpeedMetersPerSecond = 10f,
                JumpDriveChargePercent = 75f,
                JumpDriveReady = false,
            };

            var state = FlightHudPresenter.Build(telemetry);

            Assert.AreEqual(0.75f, state.JumpChargeFill01, 0.001f);
            Assert.IsFalse(state.JumpReady);
            Assert.AreEqual("JUMP 75%", state.JumpStatusText);
        }

        [Test]
        public void Build_JumpReady_ShowsReadyText()
        {
            var telemetry = new StubTelemetry
            {
                SpeedMetersPerSecond = 10f,
                JumpDriveChargePercent = 100f,
                JumpDriveReady = true,
            };

            var state = FlightHudPresenter.Build(telemetry);

            Assert.IsTrue(state.JumpReady);
            Assert.AreEqual("JUMP RDY", state.JumpStatusText);
        }
    }
}
