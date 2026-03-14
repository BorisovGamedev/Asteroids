using Asteroids.Configs;
using Asteroids.Core;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Asteroids.App
{
    public class GameBootstrapper : IInitializable
    {
        private readonly IConfigProvider _configProvider;
        private readonly GameStateMachine _stateMachine;

        public GameBootstrapper(IConfigProvider configProvider, GameStateMachine stateMachine)
        {
            _configProvider = configProvider;
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            _stateMachine.ChangeState(GameState.Bootstrap);
            LoadGameAsync().Forget();
        }

        private async UniTaskVoid LoadGameAsync()
        {
            UnityEngine.Debug.Log("Загрузка конфигов...");
            await _configProvider.LoadAllConfigsAsync();
            UnityEngine.Debug.Log($"Конфиги загружены! Здоровье игрока: {_configProvider.Player.MaxHealth}");

            _stateMachine.ChangeState(GameState.MainMenu);
            
            SceneManager.LoadScene("Game"); 
        }
    }
}