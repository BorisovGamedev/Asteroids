using Asteroids.Entities;
using Asteroids.Entities.Weapons;
using Asteroids.Entities.Enemies;
using Asteroids.Entities.Enemies.Ufo;
using Asteroids.InputService;
using Asteroids.Physics;
using Asteroids.Core;
using Asteroids.UI;
using UnityEngine;
using Zenject;

namespace Asteroids.App
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _playerPrefab;
        [SerializeField] private BulletView _bulletPrefab;
        [SerializeField] private AsteroidView _asteroidPrefab;
        [SerializeField] private UfoView _ufoPrefab;
        [SerializeField] private VirtualJoystick _virtualJoystick;

        public override void InstallBindings()
        {
            Container.Bind<ScreenWrapService>().AsSingle();

            Container.Bind<PlayerView>()
                .FromComponentInNewPrefab(_playerPrefab)
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<WeaponService>().AsSingle()
                .WithArguments(_bulletPrefab);
            
            Container.Bind<EnemyFactory>().AsSingle()
                .WithArguments(_asteroidPrefab, _ufoPrefab);
            
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<HudViewModel>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<GameSystemFacade>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameOverViewModel>().AsSingle();
            
            if (!Container.HasBinding<IInputService>())
            {
                Container.Bind<IInputService>().To<MobileInputService>().AsSingle()
                    .WithArguments(_virtualJoystick);
            }
        }
    }
}