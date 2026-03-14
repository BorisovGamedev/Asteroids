using Asteroids.Entities;
using Asteroids.Entities.Weapons;
using Asteroids.Entities.Enemies;
using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.App
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _playerPrefab;
        [SerializeField] private BulletView _bulletPrefab;
        [SerializeField] private AsteroidView _asteroidPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ScreenWrapService>().AsSingle();

            Container.Bind<PlayerView>()
                .FromComponentInNewPrefab(_playerPrefab)
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<WeaponService>().AsSingle()
                .WithArguments(_bulletPrefab);
            
            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle()
                .WithArguments(_asteroidPrefab);
        }
    }
}