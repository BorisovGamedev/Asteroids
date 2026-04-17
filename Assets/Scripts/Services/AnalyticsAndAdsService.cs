using System;
using Asteroids.Core;
using Firebase;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;

namespace Asteroids.Services
{
    public class AnalyticsAndAdsService : IInitializable, IDisposable, 
        IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private const string AndroidGameId = "1234567";
        private const string IosGameId = "1234568";
        private const string AdUnitId = "Interstitial_Android";
        private const bool TestMode = true;

        private readonly SignalBus _signalBus;
        private readonly ScoreManager _scoreManager;

        private bool _isFirebaseReady = false;

        public AnalyticsAndAdsService(SignalBus signalBus, ScoreManager scoreManager)
        {
            _signalBus = signalBus;
            _scoreManager = scoreManager;
        }

        public void Initialize()
        {
            InitializeFirebase();
            InitializeAds();

            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        private void InitializeFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _isFirebaseReady = true;
                    Debug.Log("Firebase инициализирован!");
                    LogEvent("game_started");
                }
            });
        }

        private void LogEvent(string eventName, string parameterName = null, int parameterValue = 0)
        {
            if (!_isFirebaseReady) return;

            if (parameterName == null) FirebaseAnalytics.LogEvent(eventName);
            else FirebaseAnalytics.LogEvent(eventName, new Parameter(parameterName, parameterValue));
        }

        private void OnPlayerDied()
        {
            LogEvent("player_died", "score", _scoreManager.CurrentScore);

            ShowAd();
        }

        private void InitializeAds()
        {
            string gameId = Application.platform == RuntimePlatform.IPhonePlayer ? IosGameId : AndroidGameId;
            Advertisement.Initialize(gameId, TestMode, this);
        }

        private void ShowAd()
        {
            if (Advertisement.isInitialized)
            {
                Debug.Log("Начинаем загрузку рекламы перед показом...");
                Advertisement.Load(AdUnitId, this);
            }
            else
            {
                Debug.LogWarning("Реклама еще не инициализирована.");
            }
        }

        public void OnInitializationComplete() => Debug.Log("Unity Ads инициализирован!");
        public void OnInitializationFailed(UnityAdsInitializationError error, string message) => Debug.LogError($"Ошибка инициализации Ads: {message}");

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log($"Реклама {adUnitId} успешно загружена. Показываем!");
            Advertisement.Show(adUnitId, this);
        }
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message) => Debug.Log($"Ошибка загрузки рекламы: {message}");

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message) => Debug.Log($"Ошибка показа: {message}");
        public void OnUnityAdsShowStart(string adUnitId) => Debug.Log("Реклама началась на экране");
        public void OnUnityAdsShowClick(string adUnitId) => Debug.Log("Клик по рекламе");
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) => Debug.Log("Реклама закрыта игроком");
    }
}