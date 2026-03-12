using Asteroids.Configs;
using Asteroids.Core;
using Zenject;

namespace Asteroids.App
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
            
            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
            
            Container.Bind<GameStateMachine>().AsSingle();
        }
    }
}