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
            Player = await LoadJsonAsync<PlayerConfig>("PlayerConfig.json");
            Enemies = await LoadJsonAsync<EnemiesConfig>("EnemiesConfig.json");
            World = await LoadJsonAsync<WorldConfig>("WorldConfig.json");
        }

        private async UniTask<T> LoadJsonAsync<T>(string fileName)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
            string jsonText;

            // Если это Android или WebGL, используем UnityWebRequest
            if (filePath.Contains("://") || filePath.Contains(":///"))
            {
                using var request = UnityWebRequest.Get(filePath);
                await request.SendWebRequest().WithCancellation(default);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to load config {fileName}: {request.error}");
                    return default;
                }
                
                jsonText = request.downloadHandler.text;
            }
            else
            {
                // Для Windows, Mac, Editor
                jsonText = await File.ReadAllTextAsync(filePath);
            }

            return JsonConvert.DeserializeObject<T>(jsonText);
        }
    }
}