using Asteroids.Configs;
using Asteroids.Core;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Asteroids.App
{
    public class GameBootstrapper : IInitializable
    {
        private readonly IConfigProvider _configProvider;
        private readonly GameStateMachine _stateMachine;

        // Zenject сам подставит сюда зависимости через конструктор
        public GameBootstrapper(IConfigProvider configProvider, GameStateMachine stateMachine)
        {
            _configProvider = configProvider;
            _stateMachine = stateMachine;
        }

        // Этот метод вызывается автоматически при старте сцены
        public void Initialize()
        {
            _stateMachine.ChangeState(GameState.Bootstrap);
            LoadGameAsync().Forget(); // .Forget() позволяет запустить асинхронную задачу без ожидания
        }

        private async UniTaskVoid LoadGameAsync()
        {
            UnityEngine.Debug.Log("Загрузка конфигов...");
            
            // Ждем, пока распарсятся JSON файлы
            await _configProvider.LoadAllConfigsAsync();
            
            UnityEngine.Debug.Log($"Конфиги загружены! Здоровье игрока: {_configProvider.Player.MaxHealth}");

            // Переходим в главное меню
            _stateMachine.ChangeState(GameState.MainMenu);
        }
    }
}