using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace Asteroids.Configs
{
    public class ConfigProvider : IConfigProvider
    {
        public PlayerConfig Player { get; private set; }
        public EnemiesConfig Enemies { get; private set; }
        public WorldConfig World { get; private set; }

        public async UniTask LoadAllConfigsAsync()
        {
            var (player, enemies, world) = await UniTask.WhenAll(
                LoadJsonAsync<PlayerConfig>("PlayerConfig.json"),
                LoadJsonAsync<EnemiesConfig>("EnemiesConfig.json"),
                LoadJsonAsync<WorldConfig>("WorldConfig.json")
            );

            Player = player;
            Enemies = enemies;
            World = world;
        }

        private async UniTask<T> LoadJsonAsync<T>(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string jsonText;

            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                using var request = UnityWebRequest.Get(filePath);
                await request.SendWebRequest().WithCancellation(default);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception($"Failed to load config {fileName}: {request.error}");
                }
                jsonText = request.downloadHandler.text;
            }
            else
            {
                if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");
                jsonText = await File.ReadAllTextAsync(filePath);
            }

            return JsonConvert.DeserializeObject<T>(jsonText);
        }
    }
}