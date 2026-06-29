using IronExiles.Combat;
using IronExiles.Combat.AI;
using NUnit.Framework;
using UnityEngine;

namespace IronExiles.Core.Tests
{
    public sealed class NPCBrainTests
    {
        [Test]
        public void NPCSettings_beam_dps_is_less_than_player_default()
        {
            // BR-3: NPC weapon DPS <= player starter beam
            Assert.That(NPCSettings.BeamDps, Is.LessThanOrEqualTo(BeamWeaponSettings.DefaultTier1BaseDps));
        }

        [Test]
        public void NPCSettings_disengage_radius_is_double_aggro()
        {
            Assert.That(NPCSettings.DisengageRadius, Is.EqualTo(NPCSettings.AggroRadius * 2f));
        }

        [Test]
        public void NPCBrainState_has_expected_values()
        {
            Assert.That((int)NPCBrainState.Idle, Is.EqualTo(0));
            Assert.That((int)NPCBrainState.Patrol, Is.EqualTo(1));
            Assert.That((int)NPCBrainState.Combat, Is.EqualTo(2));
            Assert.That((int)NPCBrainState.Dead, Is.EqualTo(3));
        }

        [Test]
        public void NPCBrain_initializes_to_patrol_on_server()
        {
            var go = new GameObject("TestNPC");
            go.AddComponent<TargetableEntity>();
            go.AddComponent<NetworkDamageableHealth>();
            go.AddComponent<NetworkShipTargetingController>();
            go.AddComponent<NetworkShipBeamWeaponController>();
            var brain = go.AddComponent<NPCBrain>();

            brain.Initialize(Vector3.zero, isServer: true);

            Assert.That(brain.CurrentState, Is.EqualTo(NPCBrainState.Patrol));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void NPCBrain_stays_idle_when_not_server()
        {
            var go = new GameObject("TestNPC");
            go.AddComponent<TargetableEntity>();
            go.AddComponent<NetworkDamageableHealth>();
            go.AddComponent<NetworkShipTargetingController>();
            go.AddComponent<NetworkShipBeamWeaponController>();
            var brain = go.AddComponent<NPCBrain>();

            brain.Initialize(Vector3.zero, isServer: false);

            Assert.That(brain.CurrentState, Is.EqualTo(NPCBrainState.Idle));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void NPCBrain_state_changed_event_fires_on_transition()
        {
            var go = new GameObject("TestNPC");
            go.AddComponent<TargetableEntity>();
            go.AddComponent<NetworkDamageableHealth>();
            go.AddComponent<NetworkShipTargetingController>();
            go.AddComponent<NetworkShipBeamWeaponController>();
            var brain = go.AddComponent<NPCBrain>();

            NPCBrainState? fromState = null;
            NPCBrainState? toState = null;
            brain.StateChanged += (from, to) =>
            {
                fromState = from;
                toState = to;
            };

            brain.Initialize(Vector3.zero, isServer: true);

            Assert.That(fromState, Is.EqualTo(NPCBrainState.Idle));
            Assert.That(toState, Is.EqualTo(NPCBrainState.Patrol));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void NPCBrain_spawn_origin_is_set()
        {
            var go = new GameObject("TestNPC");
            go.AddComponent<TargetableEntity>();
            go.AddComponent<NetworkDamageableHealth>();
            go.AddComponent<NetworkShipTargetingController>();
            go.AddComponent<NetworkShipBeamWeaponController>();
            var brain = go.AddComponent<NPCBrain>();

            var origin = new Vector3(100f, 20f, 50f);
            brain.Initialize(origin, isServer: true);

            Assert.That(brain.SpawnOrigin, Is.EqualTo(origin));

            Object.DestroyImmediate(go);
        }

        [Test]
        public void NPCSettings_spawn_count_defaults_to_three()
        {
            // BR-4: configurable count, default 3
            Assert.That(NPCSettings.DefaultSpawnCount, Is.EqualTo(3));
        }

        [Test]
        public void NPCSettings_respawn_delay_defaults_to_sixty_seconds()
        {
            // BR-5: respawn after configurable delay (default 60s)
            Assert.That(NPCSettings.RespawnDelaySeconds, Is.EqualTo(60f));
        }

        [Test]
        public void NPC_hull_is_less_than_player_default()
        {
            // NPC should be defeatable by player
            Assert.That(NPCSettings.MaxHull, Is.LessThan(BeamWeaponSettings.DefaultMaxHull));
        }

        [Test]
        public void NPC_damage_application_triggers_death()
        {
            // Simulate NPC taking lethal damage - verify math works correctly
            var currentHull = NPCSettings.MaxHull;
            var dps = BeamWeaponSettings.DefaultTier1BaseDps;
            var timeToKill = currentHull / dps;

            // At player DPS (250), NPC with 500 hull dies in 2 seconds
            Assert.That(timeToKill, Is.EqualTo(2f).Within(0.01f));

            // Apply lethal damage via math
            var result = DamageableHealthMath.ApplyDamage(currentHull, currentHull + 100f, out var destroyed);
            Assert.That(result, Is.EqualTo(0f));
            Assert.That(destroyed, Is.True);
        }
    }
}
