using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.InputService;
using Zenject;

namespace Asteroids.App
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<LaserFiredSignal>();
            Container.DeclareSignal<EnemyKilledSignal>();
            Container.DeclareSignal<PlayerDiedSignal>();

            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
            Container.Bind<GameStateMachine>().AsSingle();
            Container.Bind<IInputService>().To<DesktopInputService>().AsSingle();
        }
    }
}