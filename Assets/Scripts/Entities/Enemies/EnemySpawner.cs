using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Entities.Weapons;
using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Enemies
{
    public class EnemySpawner : ITickable
    {
        private readonly IInstantiator _instantiator;
        private readonly AsteroidView _asteroidPrefab;
        private readonly EnemiesConfig _enemiesConfig;
        private readonly WorldConfig _worldConfig;
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerController _player;
        private readonly WeaponService _weaponService;
        
        private readonly CustomObjectPool<Asteroid> _asteroidPool;
        private float _spawnTimer;
        
        public EnemySpawner(
            IInstantiator instantiator,
            AsteroidView asteroidPrefab,
            IConfigProvider configProvider,
            ScreenWrapService screenWrap,
            PlayerController player,
            WeaponService weaponService)
        {
            _instantiator = instantiator;
            _asteroidPrefab = asteroidPrefab;
            _enemiesConfig = configProvider.Enemies;
            _worldConfig = configProvider.World;
            _screenWrap = screenWrap;
            _player = player;
            _weaponService = weaponService;

            _asteroidPool = new CustomObjectPool<Asteroid>(
                createFunc: () => new Asteroid(_instantiator.InstantiatePrefabForComponent<AsteroidView>(_asteroidPrefab)),
                actionOnGet: a => a.View.GameObject.SetActive(true),
                actionOnRelease: a => a.View.GameObject.SetActive(false)
            );
        }
        
        public void Tick()
        {
            HandleSpawning();
            UpdateMovement();
            CheckCollisions();
        }
        
        private void HandleSpawning()
        {
            _spawnTimer += Time.deltaTime;
            
            if (_spawnTimer >= _worldConfig.SpawnDelaySeconds && _asteroidPool.ActiveItems.Count < _worldConfig.MaxEnemiesOnScreen)
            {
                _spawnTimer = 0f;
                SpawnAsteroid(size: 2);
            }
        }
        
        private void SpawnAsteroid(int size, Vector2? specificPosition = null)
        {
            Asteroid asteroid = _asteroidPool.Get();
            
            Vector2 spawnPos = specificPosition ?? GetRandomPositionOnEdge();
            
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            
            float baseSpeed = Random.Range(_enemiesConfig.AsteroidMinSpeed, _enemiesConfig.AsteroidMaxSpeed);
            float finalSpeed = size == 2 ? baseSpeed : baseSpeed * _enemiesConfig.AsteroidFragmentSpeedMultiplier;

            asteroid.Launch(spawnPos, randomDirection, finalSpeed, size, _enemiesConfig);
        }
        
        private void UpdateMovement()
        {
            float deltaTime = Time.deltaTime;
            
            foreach (var asteroid in _asteroidPool.ActiveItems)
            {
                asteroid.PhysicsBody.UpdateState(deltaTime);
                
                _screenWrap.Wrap(asteroid.PhysicsBody);

                asteroid.View.Transform.position = asteroid.PhysicsBody.Position;
                
                asteroid.View.Transform.Rotate(Vector3.forward, 45f * deltaTime);
            }
        }

        private void CheckCollisions()
        {
            List<Asteroid> asteroidsToDestroy = new List<Asteroid>();
            List<Bullet> bulletsToDestroy = new List<Bullet>();

            foreach (var asteroid in _asteroidPool.ActiveItems)
            {
                if (_player.PhysicsBody.IsCollidingWith(asteroid.PhysicsBody))
                {
                    Debug.Log("Астероид ударил игрока!");

                    _player.PhysicsBody.BounceOff(asteroid.PhysicsBody);
                }

                foreach (var bullet in _weaponService.GetActiveBullets())
                {
                    if (bullet.PhysicsBody.IsCollidingWith(asteroid.PhysicsBody))
                    {
                        asteroidsToDestroy.Add(asteroid);
                        bulletsToDestroy.Add(bullet);
                        break;
                    }
                }
            }
            
            foreach (var bullet in bulletsToDestroy)
            {
                _weaponService.ReleaseBullet(bullet);
            }

            foreach (var asteroid in asteroidsToDestroy)
            {
                if (asteroid.Size == 2)
                {
                    for (int i = 0; i < _enemiesConfig.AsteroidFragmentsCount; i++)
                    {
                        SpawnAsteroid(size: 1, asteroid.PhysicsBody.Position);
                    }
                }
                
                _asteroidPool.Release(asteroid);
            }
        }
        
        private Vector2 GetRandomPositionOnEdge()
        {
            float w = _worldConfig.WorldWidth / 2f;
            float h = _worldConfig.WorldHeight / 2f;

            if (Random.value > 0.5f)
            {
                float x = Random.value > 0.5f ? w : -w;
                float y = Random.Range(-h, h);
                return new Vector2(x, y);
            }
            else
            {
                float x = Random.Range(-w, w);
                float y = Random.value > 0.5f ? h : -h;
                return new Vector2(x, y);
            }
        }
    }
}