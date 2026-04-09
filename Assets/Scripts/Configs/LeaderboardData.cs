using System;
using System.Collections.Generic;

namespace Asteroids.Configs
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
    }

    [Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> Entries { get; set; } = new List<LeaderboardEntry>();
    }
}