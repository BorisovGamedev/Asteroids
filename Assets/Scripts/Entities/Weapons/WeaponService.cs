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
    public class WeaponService : ITickable
    {
        private const float SecondsToMilliseconds = 1000f;
        
        private readonly IInstantiator _instantiator;
        private readonly BulletView _bulletPrefab;
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerConfig _config;
        
        private readonly CustomObjectPool<Bullet> _bulletPool;

        private float _lastFireTime;

        public WeaponService(
            IInstantiator instantiator,
            BulletView bulletPrefab,
            IConfigProvider configProvider,
            ScreenWrapService screenWrap)
        {
            _instantiator = instantiator;
            _bulletPrefab = bulletPrefab;
            _config = configProvider.Player;
            _screenWrap = screenWrap;

            _bulletPool = new CustomObjectPool<Bullet>(
                createFunc: CreateBullet,
                actionOnGet: b => b.View.GameObject.SetActive(true),
                actionOnRelease: b => b.View.GameObject.SetActive(false)
            );
        }
        
        public IReadOnlyList<Bullet> GetActiveBullets() => _bulletPool.ActiveItems;

        private Bullet CreateBullet()
        {
            var view = _instantiator.InstantiatePrefabForComponent<BulletView>(_bulletPrefab);
            return new Bullet(view, _config.BulletRadius);
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

            foreach (var bullet in _bulletPool.ActiveItems)
            {
                bullet.PhysicsBody.UpdateState(deltaTime);
                _screenWrap.Wrap(bullet.PhysicsBody);
                
                bullet.View.Transform.position = bullet.PhysicsBody.Position;
                bullet.View.Transform.rotation = Quaternion.Euler(0, 0, bullet.PhysicsBody.Rotation);
            }
        }
    }
}