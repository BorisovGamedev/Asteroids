using System;
using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Entities.Weapons;
using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Enemies
{
    public class EnemySpawner : ITickable, IDisposable
    {
        private readonly EnemiesConfig _enemiesConfig;
        private readonly WorldConfig _worldConfig;
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerController _player;
        private readonly WeaponService _weaponService;
        private readonly EnemyFactory _factory;

        private readonly CustomObjectPool<IEnemy> _enemyPool;
        
        private SignalBus _signalBus;
        
        private float _spawnTimer;

        public EnemySpawner(
            IConfigProvider configProvider,
            ScreenWrapService screenWrap,
            PlayerController player,
            WeaponService weaponService,
            EnemyFactory factory,
            SignalBus signalBus)
        {
            _enemiesConfig = configProvider.Enemies;
            _worldConfig = configProvider.World;
            _screenWrap = screenWrap;
            _player = player;
            _weaponService = weaponService;
            _factory = factory;
            _signalBus = signalBus;
            
            _enemyPool = new CustomObjectPool<IEnemy>(
                createFunc: () => _factory.Create(EnemyType.AsteroidBig),
                actionOnGet: e => e.GameObject.SetActive(true),
                actionOnRelease: e => e.GameObject.SetActive(false)
            );
            
            signalBus.Subscribe<LaserFiredSignal>(OnLaserFired);
        }

        public void Tick()
        {
            HandleSpawning();
            UpdateMovement();
            CheckCollisions();
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<LaserFiredSignal>(OnLaserFired);
        }

        private void HandleSpawning()
        {
            _spawnTimer += Time.deltaTime;
            
            if (_spawnTimer >= _worldConfig.SpawnDelaySeconds && _enemyPool.ActiveItems.Count < _worldConfig.MaxEnemiesOnScreen)
            {
                _spawnTimer = 0f;
                
                if (UnityEngine.Random.value < _worldConfig.UfoSpawnChance)
                {
                    SpawnUfo();
                }
                else
                {
                    SpawnAsteroid(EnemyType.AsteroidBig); 
                }
            }
        }

        private void SpawnAsteroid(EnemyType type, Vector2? specificPosition = null)
        {
            IEnemy enemy = GetEnemyFromPool(type);
    
            Vector2 spawnPos = specificPosition ?? GetRandomPositionOnEdge();
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    
            float baseSpeed = UnityEngine.Random.Range(_enemiesConfig.AsteroidMinSpeed, _enemiesConfig.AsteroidMaxSpeed);
    
            float finalSpeed = type == EnemyType.AsteroidBig 
                ? baseSpeed 
                : baseSpeed * _enemiesConfig.AsteroidFragmentSpeedMultiplier;

            ((Asteroid)enemy).Launch(spawnPos, randomDirection, finalSpeed, type, _enemiesConfig);
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
                if (!_player.IsInvulnerable && !_player.IsDead)
                {
                    if (_player.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                    {
                        _player.TakeDamage(enemy.PhysicsBody.Position);
                
                        enemy.PhysicsBody.BounceOff(_player.PhysicsBody);
                    }
                }

                foreach (var bullet in _weaponService.GetActiveBullets())
                {
                    if (bullet.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                    {
                        _signalBus.Fire(new EnemyKilledSignal(enemy.Type.ToString()));
                        
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
                        SpawnAsteroid(EnemyType.AsteroidSmall, enemy.PhysicsBody.Position);
                    }
                }
                
                _enemyPool.Release(enemy);
            }
        }

        private Vector2 GetRandomPositionOnEdge()
        {
            float w = _worldConfig.WorldWidth / 2f;
            float h = _worldConfig.WorldHeight / 2f;

            if (UnityEngine.Random.value > 0.5f) return new Vector2(UnityEngine.Random.value > 0.5f ? w : -w, UnityEngine.Random.Range(-h, h));
            return new Vector2(UnityEngine.Random.Range(-w, w), UnityEngine.Random.value > 0.5f ? h : -h);
        }
        
        private void OnLaserFired(LaserFiredSignal signal)
        {
            List<IEnemy> enemiesToDestroy = new List<IEnemy>();

            foreach (var enemy in _enemyPool.ActiveItems)
            {
                if (PhysicsMath.RayIntersectsCircle(signal.Origin, signal.Direction, signal.Length, enemy.PhysicsBody.Position, enemy.PhysicsBody.Radius))
                {
                    _signalBus.Fire(new EnemyKilledSignal(enemy.Type.ToString()));
                    enemiesToDestroy.Add(enemy);
                }
            }

            foreach (var enemy in enemiesToDestroy)
            {
                _enemyPool.Release(enemy);
            }
    
            Debug.Log($"Лазер уничтожил {enemiesToDestroy.Count} врагов!");
        }
    }
}