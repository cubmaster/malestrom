namespace IronExiles.Combat.AI
{
    /// <summary>
    /// Configuration constants for NPC combat AI.
    /// All distances in meters, times in seconds.
    /// </summary>
    public static class NPCSettings
    {
        /// <summary>Range at which NPC detects and aggros on a player.</summary>
        public const float AggroRadius = 200f;

        /// <summary>Range beyond which NPC disengages and returns to patrol (2x aggro).</summary>
        public const float DisengageRadius = 400f;

        /// <summary>Maximum hull HP for NPC ships.</summary>
        public const float MaxHull = 500f;

        /// <summary>Shield HP per facing for NPC ships.</summary>
        public const float ShieldPerFacing = 100f;

        /// <summary>NPC beam weapon DPS (60% of player default 250).</summary>
        public const float BeamDps = 150f;

        /// <summary>NPC beam weapon range in meters.</summary>
        public const float BeamRange = 150f;

        /// <summary>Default number of NPCs to spawn in test sector.</summary>
        public const int DefaultSpawnCount = 3;

        /// <summary>Seconds before a dead NPC respawns.</summary>
        public const float RespawnDelaySeconds = 60f;

        /// <summary>Radius around spawn point for patrol wandering.</summary>
        public const float PatrolRadius = 100f;

        /// <summary>Fraction of max thrust used during patrol (0-1).</summary>
        public const float PatrolSpeedFraction = 0.4f;

        /// <summary>Fraction of max thrust used during combat approach.</summary>
        public const float CombatSpeedFraction = 0.7f;

        /// <summary>Preferred combat engagement distance from target.</summary>
        public const float EngagementDistance = 80f;

        /// <summary>Time in seconds with no LOS before NPC disengages.</summary>
        public const float LosBreakSeconds = 10f;

        /// <summary>Seconds between patrol waypoint changes.</summary>
        public const float PatrolWaypointInterval = 8f;
    }
}
