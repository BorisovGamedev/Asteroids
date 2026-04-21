using System;
using Asteroids.Core;
using Firebase;
using Firebase.Analytics;
using Zenject;

namespace Asteroids.Services
{
    public class AnalyticsService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly ScoreManager _scoreManager;
        private bool _isFirebaseReady = false;

        public AnalyticsService(SignalBus signalBus, ScoreManager scoreManager)
        {
            _signalBus = signalBus;
            _scoreManager = scoreManager;
        }

        public void Initialize()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
            {
                if (task.Result == DependencyStatus.Available)
                {
                    _isFirebaseReady = true;
                    LogEvent("game_started");
                }
            });
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Dispose() => _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);

        private void LogEvent(string eventName, string paramName = null, int paramValue = 0)
        {
            if (!_isFirebaseReady) return;
            if (paramName == null) FirebaseAnalytics.LogEvent(eventName);
            else FirebaseAnalytics.LogEvent(eventName, new Parameter(paramName, paramValue));
        }

        private void OnPlayerDied() => LogEvent("player_died", "score", _scoreManager.CurrentScore);
    }
}