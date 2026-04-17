using System.Collections.Generic;

namespace Asteroids.Configs
{
    public class WorldConfig
    {
        public float WorldWidth { get; set; }
        public float WorldHeight { get; set; }
        public int MaxEnemiesOnScreen { get; set; }
        public float SpawnDelaySeconds { get; set; }
        public Dictionary<string, int> ScoreRewards { get; set; }
        public float UfoSpawnChance { get; set; }
        public int MaxLeaderboardEntries { get; set; }
    }
}