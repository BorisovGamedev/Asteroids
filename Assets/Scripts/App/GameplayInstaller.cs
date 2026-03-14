using Asteroids.Entities;
using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.App
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private PlayerView _playerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ScreenWrapService>().AsSingle();

            Container.Bind<PlayerView>()
                .FromComponentInNewPrefab(_playerPrefab)
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();
        }
    }
}