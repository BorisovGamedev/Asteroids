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
    public class EnemyCollisionService : ITickable, IDisposable
    {
        private readonly EnemyManager _enemyManager;
        private readonly PlayerController _player;
        private readonly BulletService _bulletService;
        private readonly SignalBus _signalBus;
        
        private readonly EnemySpawner _spawner; 

        public EnemyCollisionService(
            EnemyManager enemyManager,
            PlayerController player,
            BulletService bulletService,
            SignalBus signalBus,
            EnemySpawner spawner)
        {
            _enemyManager = enemyManager;
            _player = player;
            _bulletService = bulletService;
            _signalBus = signalBus;
            _spawner = spawner;

            _signalBus.Subscribe<LaserFiredSignal>(OnLaserFired);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<LaserFiredSignal>(OnLaserFired);
        }

        public void Tick()
        {
            List<IEnemy> enemiesToDestroy = new List<IEnemy>();
            List<Bullet> bulletsToDestroy = new List<Bullet>();

            foreach (var enemy in _enemyManager.GetActiveEnemies())
            {
                if (!_player.IsInvulnerable && !_player.IsDead)
                {
                    if (_player.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                    {
                        _player.TakeDamage(enemy.PhysicsBody.Position);
                        enemy.PhysicsBody.BounceOff(_player.PhysicsBody);
                    }
                }

                foreach (var bullet in _bulletService.GetActiveBullets())
                {
                    if (bullet.PhysicsBody.IsCollidingWith(enemy.PhysicsBody))
                    {
                        _signalBus.Fire(new EnemyKilledSignal(enemy.Type));
                        
                        enemiesToDestroy.Add(enemy);
                        bulletsToDestroy.Add(bullet);
                        break; 
                    }
                }
            }

            foreach (var bullet in bulletsToDestroy) _bulletService.ReleaseBullet(bullet);
            
            foreach (var enemy in enemiesToDestroy)
            {
                _spawner.HandleAsteroidDestruction(enemy);
                _enemyManager.ReleaseEnemy(enemy);
            }
        }

        private void OnLaserFired(LaserFiredSignal signal)
        {
            List<IEnemy> enemiesToDestroy = new List<IEnemy>();

            foreach (var enemy in _enemyManager.GetActiveEnemies())
            {
                if (PhysicsMath.RayIntersectsCircle(signal.Origin, signal.Direction, signal.Length, enemy.PhysicsBody.Position, enemy.PhysicsBody.Radius))
                {
                    _signalBus.Fire(new EnemyKilledSignal(enemy.Type));
                    enemiesToDestroy.Add(enemy);
                }
            }
            
            foreach (var enemy in enemiesToDestroy)
            {
                _enemyManager.ReleaseEnemy(enemy);
            }
        }
    }
}