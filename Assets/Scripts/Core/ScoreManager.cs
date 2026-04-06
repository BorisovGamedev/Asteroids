using System;
using Asteroids.Configs;
using Zenject;

namespace Asteroids.Core
{
    public class ScoreManager : IInitializable, IDisposable
    {
        public int CurrentScore { get; private set; }
        
        private readonly SignalBus _signalBus;
        private readonly WorldConfig _worldConfig;

        public ScoreManager(SignalBus signalBus, IConfigProvider configProvider)
        {
            _signalBus = signalBus;
            _worldConfig = configProvider.World;
        }

        public void Initialize()
        {
            CurrentScore = 0;

            _signalBus.Subscribe<EnemyKilledSignal>(OnEnemyKilled);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<EnemyKilledSignal>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledSignal signal)
        {
            if (_worldConfig.ScoreRewards.TryGetValue(signal.EnemyTypeStr, out int reward))
            {
                CurrentScore += reward;
            }
        }
    }
}