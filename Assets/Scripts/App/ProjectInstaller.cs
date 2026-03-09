using Asteroids.Configs;
using Zenject;

namespace Asteroids.App
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Биндим ConfigProvider как IConfigProvider в единственном экземпляре (Single)
            Container.Bind<IConfigProvider>().To<ConfigProvider>().AsSingle();
        }
    }
}