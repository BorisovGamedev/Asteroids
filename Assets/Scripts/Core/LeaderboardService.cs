using System.IO;
using System.Linq;
using Asteroids.Configs;
using Newtonsoft.Json;
using UnityEngine;

namespace Asteroids.Core
{
    public class LeaderboardService
    {
        private const int DefaultMaxEntries = 10;

        private readonly string _filePath;
        private readonly IConfigProvider _configProvider; 
        
        public LeaderboardData Data { get; private set; }

        public LeaderboardService(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
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

        private int GetValidatedMaxEntries()
        {
            if (_configProvider.World == null)
            {
                Debug.LogError($"[LeaderboardService] WorldConfig равен null! Используется дефолтное значение: {DefaultMaxEntries}");
                return DefaultMaxEntries;
            }

            int configuredMax = _configProvider.World.MaxLeaderboardEntries;

            if (configuredMax <= 0)
            {
                Debug.LogError($"[LeaderboardService] MaxLeaderboardEntries в JSON ({configuredMax}) меньше или равно нулю! Используется дефолтное значение: {DefaultMaxEntries}");
                return DefaultMaxEntries;
            }

            return configuredMax;
        }

        public void AddEntry(string playerName, int score)
        {
            int maxEntries = GetValidatedMaxEntries();

            Data.Entries.Add(new LeaderboardEntry { PlayerName = playerName, Score = score });
            
            Data.Entries = Data.Entries
                .OrderByDescending(e => e.Score)
                .Take(maxEntries)
                .ToList();
                
            Save(); 
        }

        public bool IsNewHighScore(int score)
        {
            if (score <= 0) return false;
            
            int maxEntries = GetValidatedMaxEntries();

            if (Data.Entries.Count < maxEntries) return true;
            
            return score > Data.Entries.Last().Score;
        }
    }
}