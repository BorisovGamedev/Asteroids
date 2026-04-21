using System;
using Asteroids.Core;
using UnityEngine.Advertisements;
using Zenject;

namespace Asteroids.Services
{
    public class AdsService : IInitializable, IDisposable, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private const string AndroidGameId = "1234567";
        private const string AdUnitId = "Interstitial_Android";
        private readonly SignalBus _signalBus;

        public AdsService(SignalBus signalBus) => _signalBus = signalBus;

        public void Initialize()
        {
            Advertisement.Initialize(AndroidGameId, true, this);
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Dispose() => _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);

        private void OnPlayerDied()
        {
            if (Advertisement.isInitialized) Advertisement.Load(AdUnitId, this);
        }

        public void OnUnityAdsAdLoaded(string adUnitId) => Advertisement.Show(adUnitId, this);

        // ... оставим пустые колбэки интерфейсов ...
        public void OnInitializationComplete() { }
        public void OnInitializationFailed(UnityAdsInitializationError e, string m) { }
        public void OnUnityAdsFailedToLoad(string id, UnityAdsLoadError e, string m) { }
        public void OnUnityAdsShowFailure(string id, UnityAdsShowError e, string m) { }
        public void OnUnityAdsShowStart(string id) { }
        public void OnUnityAdsShowClick(string id) { }
        public void OnUnityAdsShowComplete(string id, UnityAdsShowCompletionState s) { }
    }
}