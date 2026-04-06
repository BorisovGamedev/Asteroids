using Asteroids.Configs;
using Asteroids.Entities.Enemies.Ufo;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Enemies
{
    public class EnemyFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly AsteroidView _asteroidPrefab;
        private readonly UfoView _ufoPrefab;
        private readonly PlayerController _player;
        private readonly EnemiesConfig _config;

        public EnemyFactory(
            IInstantiator instantiator, 
            AsteroidView asteroidPrefab, 
            UfoView ufoPrefab, 
            PlayerController player,
            IConfigProvider configProvider)
        {
            _instantiator = instantiator;
            _asteroidPrefab = asteroidPrefab;
            _ufoPrefab = ufoPrefab;
            _player = player;
            _config = configProvider.Enemies;
        }

        public IEnemy Create(EnemyType type)
        {
            switch (type)
            {
                case EnemyType.AsteroidBig:
                case EnemyType.AsteroidSmall:
                    var asteroidView = _instantiator.InstantiatePrefabForComponent<AsteroidView>(_asteroidPrefab);
                    return new Asteroid(asteroidView, _config);
                    
                case EnemyType.Ufo:
                    var ufoView = _instantiator.InstantiatePrefabForComponent<UfoView>(_ufoPrefab);
                    return new UfoEnemy(ufoView, _player, _config);
                    
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}