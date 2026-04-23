using Asteroids.Configs;
using Asteroids.Core;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Asteroids.App
{
    public class GameBootstrapper : IInitializable
    {
        public const string GameSceneName = "Game";
        
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

        private async UniTask LoadGameAsync()
        {
            await _configProvider.LoadAllConfigsAsync();
            _stateMachine.ChangeState(GameState.MainMenu);
            SceneManager.LoadScene(GameSystemFacade.GameSceneName); 
        }
    }
}