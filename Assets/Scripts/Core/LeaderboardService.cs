using System.IO;
using System.Linq;
using Asteroids.Configs;
using Newtonsoft.Json;
using UnityEngine;

namespace Asteroids.Core
{
    public class LeaderboardService
    {
        private readonly int _maxEntries;
        private readonly string _filePath;
        
        public LeaderboardData Data { get; private set; }

        public LeaderboardService(IConfigProvider configProvider)
        {
            _maxEntries = configProvider.World.MaxLeaderboardEntries;
            _filePath = Path.Combine(Application.persistentDataPath, "Leaderboard.json");
            Load();
        }

        private void Load()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Data = JsonConvert.DeserializeObject<LeaderboardData>(json) ?? new LeaderboardData();
            }
            else
            {
                Data = new LeaderboardData();
            }
        }

        private void Save()
        {
            string json = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddEntry(string playerName, int score)
        {
            Data.Entries.Add(new LeaderboardEntry { PlayerName = playerName, Score = score });
            
            Data.Entries = Data.Entries
                .OrderByDescending(e => e.Score)
                .Take(_maxEntries)
                .ToList();
                
            Save();
        }

        public void ClearLeaderboard()
        {
            Data.Entries.Clear();
            Save();
        }

        public bool IsNewHighScore(int score)
        {
            if (score <= 0) return false;
            if (Data.Entries.Count < _maxEntries) return true;
            
            return score > Data.Entries.Last().Score;
        }
    }
}