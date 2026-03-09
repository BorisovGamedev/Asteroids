using Asteroids.Configs;
using Zenject;

namespace Asteroids.App
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
        }
    }
}