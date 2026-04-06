using System;
using System.Threading;
using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Physics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Weapons
{
    public class WeaponService : ITickable, IDisposable
    {
        private const float SecondsToMilliseconds = 1000f;
        
        private readonly IInstantiator _instantiator;
        private readonly BulletView _bulletPrefab;
        private readonly PlayerConfig _config;
        private readonly ScreenWrapService _screenWrap;
        
        private readonly SignalBus _signalBus;
        private readonly PlayerView _playerView;
        
        private readonly CustomObjectPool<Bullet> _bulletPool;
        private float _lastFireTime;

        public int CurrentLaserCharges { get; private set; }
        public float LaserCooldownTimer { get; private set; }

        public WeaponService(
            IInstantiator instantiator,
            BulletView bulletPrefab,
            IConfigProvider configProvider,
            ScreenWrapService screenWrap,
            SignalBus signalBus,
            PlayerView playerView)
        {
            _instantiator = instantiator;
            _bulletPrefab = bulletPrefab;
            _config = configProvider.Player;
            _screenWrap = screenWrap;
            _signalBus = signalBus; 
            _playerView = playerView;

            _bulletPool = new CustomObjectPool<Bullet>(
                createFunc: CreateBullet,
                actionOnGet: b => b.View.GameObject.SetActive(true),
                actionOnRelease: b => b.View.GameObject.SetActive(false)
            );
            
            CurrentLaserCharges = _config.MaxLaserCharges;
            LaserCooldownTimer = 0f;
        }
        
        public IReadOnlyList<Bullet> GetActiveBullets() => _bulletPool.ActiveItems;

        private Bullet CreateBullet()
        {
            var view = _instantiator.InstantiatePrefabForComponent<BulletView>(_bulletPrefab);
            return new Bullet(view, _config);
        }

        public void Fire(Vector2 spawnPosition, float rotation, Vector2 forwardDirection)
        {
            if (Time.time - _lastFireTime < _config.FireRateSeconds) return;
            _lastFireTime = Time.time;
            
            Bullet bullet = _bulletPool.Get();
            
            float spawnOffset = _config.PlayerRadius + _config.BulletRadius + _config.BulletSpawnOffset;
            bullet.PhysicsBody.Position = spawnPosition + (forwardDirection * spawnOffset);
            
            bullet.PhysicsBody.Rotation = rotation;
            bullet.PhysicsBody.Stop();
            bullet.PhysicsBody.AddForce(forwardDirection * _config.BulletSpeed, 1f);
            
            bullet.Cts?.Cancel();
            bullet.Cts?.Dispose();
            bullet.Cts = new CancellationTokenSource();
            
            DeactivateBulletAfterTimeAsync(bullet, bullet.Cts.Token).Forget();
        }

        private async UniTaskVoid DeactivateBulletAfterTimeAsync(Bullet bullet, CancellationToken token)
        {
            int delayMilliseconds = Mathf.RoundToInt(_config.BulletLifeTimeSeconds * SecondsToMilliseconds);

            bool isCancelled = await UniTask.Delay(delayMilliseconds, cancellationToken: token).SuppressCancellationThrow();

            if (isCancelled) return;

            ReleaseBullet(bullet);
        }
        
        public void ReleaseBullet(Bullet bullet)
        {
            bullet.Cts?.Cancel(); 
    
            _bulletPool.Release(bullet);
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;

            if (CurrentLaserCharges < _config.MaxLaserCharges)
            {
                LaserCooldownTimer += deltaTime;
                if (LaserCooldownTimer >= _config.LaserCooldownSeconds)
                {
                    CurrentLaserCharges++;
                    LaserCooldownTimer = 0f;
                    Debug.Log($"Лазер заряжен! Зарядов: {CurrentLaserCharges}");
                }
            }
            
            foreach (var bullet in _bulletPool.ActiveItems)
            {
                bullet.PhysicsBody.UpdateState(deltaTime);
                _screenWrap.Wrap(bullet.PhysicsBody);
                
                bullet.View.Transform.position = bullet.PhysicsBody.Position;
                bullet.View.Transform.rotation = Quaternion.Euler(0, 0, bullet.PhysicsBody.Rotation);
            }
        }
        
        public void FireLaser(Vector2 origin, Vector2 direction)
        {
            if (CurrentLaserCharges <= 0) return;

            CurrentLaserCharges--;
            LaserCooldownTimer = 0f;
            
            float laserLength = 30f;
            
            float spawnOffset = _config.PlayerRadius + _config.BulletSpawnOffset;
            
            Vector2 startPosition = origin + (direction * spawnOffset);
            
            _signalBus.Fire(new LaserFiredSignal(startPosition, direction, laserLength));

            ShowLaserVisualAsync(startPosition, direction, laserLength).Forget();
        }
        
        public void Dispose()
        {
            foreach (var bullet in _bulletPool.ActiveItems)
            {
                bullet.Cts?.Cancel();
                bullet.Cts?.Dispose();
            }
        }
        
        private async UniTaskVoid ShowLaserVisualAsync(Vector2 origin, Vector2 direction, float length)
        {
            _playerView.LaserLine.SetPosition(0, origin);
            _playerView.LaserLine.SetPosition(1, origin + (direction * length));
            _playerView.LaserLine.enabled = true;

            await UniTask.Delay(200);

            _playerView.LaserLine.enabled = false;
        }
    }
}