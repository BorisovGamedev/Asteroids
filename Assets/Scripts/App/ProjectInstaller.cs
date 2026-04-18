using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.InputService;
using Asteroids.Services;
using UnityEngine;
using Zenject;

namespace Asteroids.App
{
    public class ProjectInstaller : MonoInstaller
    {
        [Header("Debug Controls")]
        [Tooltip("Поставь галочку, чтобы в Unity Editor появился мобильный джойстик")]
        public bool ForceMobileInputInEditor = false;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<LaserFiredSignal>();
            Container.DeclareSignal<EnemyKilledSignal>();
            Container.DeclareSignal<PlayerDiedSignal>();

            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<LeaderboardService>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<AnalyticsAndAdsService>().AsSingle();

            bool useMobile = false;

#if UNITY_ANDROID || UNITY_IOS
            useMobile = true;
#endif

#if UNITY_EDITOR
            useMobile = ForceMobileInputInEditor;
#endif

            if (!useMobile)
            {
                Container.Bind<IInputService>().To<DesktopInputService>().AsSingle();
            }
        }
    }
}