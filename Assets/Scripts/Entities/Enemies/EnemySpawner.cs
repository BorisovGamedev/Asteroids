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
        private readonly EnemiesConfig _enemiesConfig;
        private readonly WorldConfig _worldConfig;
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerController _player;
        private readonly WeaponService _weaponService;
        private readonly EnemyFactory _factory;

        private readonly CustomObjectPool<IEnemy> _enemyPool;
        
        private float _spawnTimer;

        public EnemySpawner(
            IConfigProvider configProvider,
            ScreenWrapService screenWrap,
            PlayerController player,
            WeaponService weaponService,
            EnemyFactory factory)
        {
            _enemiesConfig = configProvider.Enemies;
            _worldConfig = configProvider.World;
            _screenWrap = screenWrap;
            _player = player;
            _weaponService = weaponService;
            _factory = factory;
            
            _enemyPool = new CustomObjectPool<IEnemy>(
                createFunc: () => _factory.Create(EnemyType.AsteroidBig),
                actionOnGet: e => e.GameObject.SetActive(true),
                actionOnRelease: e => e.GameObject.SetActive(false)
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
            
            if (_spawnTimer >= _worldConfig.SpawnDelaySeconds && _enemyPool.ActiveItems.Count < _worldConfig.MaxEnemiesOnScreen)
            {
                _spawnTimer = 0f;
                
                if (Random.value < 0.2f) // Вынести в Json
                {
                    SpawnUfo();
                }
                else
                {
                    SpawnAsteroid(size: 2); // Вынести в Json
                }
            }
        }

        private void SpawnAsteroid(int size, Vector2? specificPosition = null)
        {
            IEnemy enemy = GetEnemyFromPool(size == 2 ? EnemyType.AsteroidBig : EnemyType.AsteroidSmall);
            
            Vector2 spawnPos = specificPosition ?? GetRandomPositionOnEdge();
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            
            float baseSpeed = Random.Range(_enemiesConfig.AsteroidMinSpeed, _enemiesConfig.AsteroidMaxSpeed);
            float finalSpeed = size == 2 ? baseSpeed : baseSpeed * _enemiesConfig.AsteroidFragmentSpeedMultiplier;

            ((Asteroid)enemy).Launch(spawnPos, randomDirection, finalSpeed, size, _enemiesConfig);
        }

        private void SpawnUfo()
        {
            IEnemy enemy = GetEnemyFromPool(EnemyType.Ufo);
            Vector2 spawnPos = GetRandomPositionOnEdge();
            
            ((Ufo.UfoEnemy)enemy).Launch(spawnPos);
        }

        private IEnemy GetEnemyFromPool(EnemyType type)
        {
            IEnemy newEnemy = _factory.Create(type);
            _enemyPool.ActiveItems.Add(newEnemy);
            newEnemy.GameObject.SetActive(true);
            return newEnemy;
        }

        private void UpdateMovement()
        {
            float dt = Time.deltaTime;
            
            foreach (var enemy in _enemyPool.ActiveItems)
            {
                enemy.Tick(dt);
                _screenWrap.Wrap(enemy.PhysicsBody);
            }
        }

        private void CheckCollisions()
        {
            List<IEnemy> enemiesToDestroy = new List<IEnemy>();
            List<Bullet> bulletsToDestroy = new List<Bullet>();

            foreach (var enemy in _enemyPool.ActiveItems)
            {
                if (_player.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                {
                    Debug.Log($"{_player.PhysicsBody.Position} ударился об {enemy.Type}");
                    _player.PhysicsBody.BounceOff(enemy.PhysicsBody);
                }

                foreach (var bullet in _weaponService.GetActiveBullets())
                {
                    if (bullet.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                    {
                        enemiesToDestroy.Add(enemy);
                        bulletsToDestroy.Add(bullet);
                        break; 
                    }
                }
            }

            foreach (var bullet in bulletsToDestroy)
            {
                _weaponService.ReleaseBullet(bullet);
            }

            foreach (var enemy in enemiesToDestroy)
            {
                if (enemy.Type == EnemyType.AsteroidBig)
                {
                    for (int i = 0; i < _enemiesConfig.AsteroidFragmentsCount; i++)
                    {
                        SpawnAsteroid(size: 1, enemy.PhysicsBody.Position);
                    }
                }
                
                _enemyPool.Release(enemy);
            }
        }

        private Vector2 GetRandomPositionOnEdge()
        {
            float w = _worldConfig.WorldWidth / 2f;
            float h = _worldConfig.WorldHeight / 2f;

            if (Random.value > 0.5f) return new Vector2(Random.value > 0.5f ? w : -w, Random.Range(-h, h));
            return new Vector2(Random.Range(-w, w), Random.value > 0.5f ? h : -h);
        }
    }
}