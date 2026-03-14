using Zenject;

namespace Asteroids.App
{
    public class BootInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameBootstrapper>().AsSingle();
        }
    }
}