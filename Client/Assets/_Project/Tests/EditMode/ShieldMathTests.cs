using IronExiles.Combat;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class ShieldMathTests
    {
        [Test]
        public void DetermineFacing_forward_returns_Front()
        {
            var facing = ShieldMath.DetermineFacing(Vector3.forward);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Front));
        }

        [Test]
        public void DetermineFacing_back_returns_Rear()
        {
            var facing = ShieldMath.DetermineFacing(Vector3.back);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Rear));
        }

        [Test]
        public void DetermineFacing_left_returns_Port()
        {
            var facing = ShieldMath.DetermineFacing(Vector3.left);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Port));
        }

        [Test]
        public void DetermineFacing_right_returns_Starboard()
        {
            var facing = ShieldMath.DetermineFacing(Vector3.right);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Starboard));
        }

        [Test]
        public void DetermineFacing_diagonal_forward_right_returns_Front()
        {
            var direction = new Vector3(0.3f, 0f, 0.9f).normalized;
            var facing = ShieldMath.DetermineFacing(direction);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Front));
        }

        [Test]
        public void DetermineFacing_diagonal_mostly_right_returns_Starboard()
        {
            var direction = new Vector3(0.9f, 0f, 0.3f).normalized;
            var facing = ShieldMath.DetermineFacing(direction);
            Assert.That(facing, Is.EqualTo(ShieldFacing.Starboard));
        }

        [Test]
        public void ComputeAbsorption_full_absorb_when_shield_exceeds_damage()
        {
            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(100f, 50f);
            Assert.That(absorbed, Is.EqualTo(50f).Within(0.001f));
            Assert.That(overflow, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ComputeAbsorption_partial_absorb_returns_overflow()
        {
            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(30f, 80f);
            Assert.That(absorbed, Is.EqualTo(30f).Within(0.001f));
            Assert.That(overflow, Is.EqualTo(50f).Within(0.001f));
        }

        [Test]
        public void ComputeAbsorption_zero_shield_returns_full_overflow()
        {
            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(0f, 60f);
            Assert.That(absorbed, Is.EqualTo(0f).Within(0.001f));
            Assert.That(overflow, Is.EqualTo(60f).Within(0.001f));
        }

        [Test]
        public void ComputeAbsorption_zero_damage_returns_zero()
        {
            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(100f, 0f);
            Assert.That(absorbed, Is.EqualTo(0f).Within(0.001f));
            Assert.That(overflow, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ComputeAbsorption_exact_match_absorbs_all()
        {
            var (absorbed, overflow) = ShieldMath.ComputeAbsorption(50f, 50f);
            Assert.That(absorbed, Is.EqualTo(50f).Within(0.001f));
            Assert.That(overflow, Is.EqualTo(0f).Within(0.001f));
        }

        [Test]
        public void ComputeRegenPerTick_zero_power_uses_half_multiplier()
        {
            var regen = ShieldMath.ComputeRegenPerTick(25f, 0f, 1f);
            Assert.That(regen, Is.EqualTo(12.5f).Within(0.001f));
        }

        [Test]
        public void ComputeRegenPerTick_half_power_uses_midpoint_multiplier()
        {
            var regen = ShieldMath.ComputeRegenPerTick(25f, 0.5f, 1f);
            Assert.That(regen, Is.EqualTo(25f * 1.25f).Within(0.001f));
        }

        [Test]
        public void ComputeRegenPerTick_full_power_uses_max_multiplier()
        {
            var regen = ShieldMath.ComputeRegenPerTick(25f, 1f, 1f);
            Assert.That(regen, Is.EqualTo(50f).Within(0.001f));
        }

        [Test]
        public void ComputeRegenPerTick_zero_base_rate_returns_zero()
        {
            var regen = ShieldMath.ComputeRegenPerTick(0f, 1f, 1f);
            Assert.That(regen, Is.EqualTo(0f));
        }

        [Test]
        public void ComputeRegenPerTick_zero_deltaTime_returns_zero()
        {
            var regen = ShieldMath.ComputeRegenPerTick(25f, 1f, 0f);
            Assert.That(regen, Is.EqualTo(0f));
        }

        [Test]
        public void ComputeRegenPerTick_scales_by_deltaTime()
        {
            var regen = ShieldMath.ComputeRegenPerTick(25f, 1f, 0.5f);
            Assert.That(regen, Is.EqualTo(25f).Within(0.001f));
        }

        [Test]
        public void GetPowerMultiplier_zero_fraction_returns_half()
        {
            var multiplier = ShieldMath.GetPowerMultiplier(0f);
            Assert.That(multiplier, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void GetPowerMultiplier_full_fraction_returns_max()
        {
            var multiplier = ShieldMath.GetPowerMultiplier(1f);
            Assert.That(multiplier, Is.EqualTo(ShieldSettings.DefaultPowerMultiplierMax).Within(0.001f));
        }

        [Test]
        public void GetPowerMultiplier_clamps_above_one()
        {
            var multiplier = ShieldMath.GetPowerMultiplier(2f);
            Assert.That(multiplier, Is.EqualTo(ShieldSettings.DefaultPowerMultiplierMax).Within(0.001f));
        }

        [Test]
        public void GetPowerMultiplier_clamps_below_zero()
        {
            var multiplier = ShieldMath.GetPowerMultiplier(-1f);
            Assert.That(multiplier, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void GetPowerMultiplier_half_fraction_returns_midpoint()
        {
            var multiplier = ShieldMath.GetPowerMultiplier(0.5f);
            Assert.That(multiplier, Is.EqualTo(1.25f).Within(0.001f));
        }

        [Test]
        public void ShieldNetworkState_Full_sets_all_facings()
        {
            var state = ShieldNetworkState.Full(200f);
            Assert.That(state.Front, Is.EqualTo(200f));
            Assert.That(state.Rear, Is.EqualTo(200f));
            Assert.That(state.Port, Is.EqualTo(200f));
            Assert.That(state.Starboard, Is.EqualTo(200f));
        }

        [Test]
        public void ShieldNetworkState_indexer_get_set()
        {
            var state = ShieldNetworkState.Full(100f);
            state[ShieldFacing.Front] = 50f;
            state[ShieldFacing.Port] = 75f;

            Assert.That(state[ShieldFacing.Front], Is.EqualTo(50f));
            Assert.That(state[ShieldFacing.Port], Is.EqualTo(75f));
            Assert.That(state[ShieldFacing.Rear], Is.EqualTo(100f));
            Assert.That(state[ShieldFacing.Starboard], Is.EqualTo(100f));
        }

        [Test]
        public void ShieldNetworkState_Equals_returns_true_for_same_values()
        {
            var a = ShieldNetworkState.Full(250f);
            var b = ShieldNetworkState.Full(250f);
            Assert.That(a.Equals(b), Is.True);
        }

        [Test]
        public void ShieldNetworkState_Equals_returns_false_for_different_values()
        {
            var a = ShieldNetworkState.Full(250f);
            var b = ShieldNetworkState.Full(250f);
            b.Front = 100f;
            Assert.That(a.Equals(b), Is.False);
        }
    }
}
