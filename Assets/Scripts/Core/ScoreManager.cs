using System;
using Asteroids.Configs;
using Zenject;

namespace Asteroids.Core
{
    public class ScoreManager : IInitializable, IDisposable
    {
        public int CurrentScore { get; private set; }
        
        private readonly SignalBus _signalBus;
        
        private readonly IConfigProvider _configProvider;
        
        private readonly GameStateMachine _stateMachine;

        public ScoreManager(SignalBus signalBus, IConfigProvider configProvider, GameStateMachine stateMachine)
        {
            _signalBus = signalBus;
            
            _configProvider = configProvider; 
            
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            CurrentScore = 0;
            _signalBus.Subscribe<EnemyKilledSignal>(OnEnemyKilled);
            _stateMachine.OnStateChanged += HandleStateChanged;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<EnemyKilledSignal>(OnEnemyKilled);
            _stateMachine.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Gameplay)
            {
                CurrentScore = 0;
            }
        }

        private void OnEnemyKilled(EnemyKilledSignal signal)
        {
            WorldConfig worldConfig = _configProvider.World;

            if (worldConfig == null || worldConfig.ScoreRewards == null)
            {
                UnityEngine.Debug.LogError("Ошибка: Конфиги не загружены!");
                return;
            }

            if (worldConfig.ScoreRewards.TryGetValue(signal.EnemyTypeStr, out int reward))
            {
                CurrentScore += reward;
            }
        }
    }
}