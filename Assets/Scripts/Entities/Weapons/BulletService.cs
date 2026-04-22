using System;
using System.Collections.Generic;
using System.Threading;
using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Physics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Weapons
{
    public class BulletService : ITickable, IDisposable
    {
        private const float SecondsToMilliseconds = 1000f;

        private readonly IInstantiator _instantiator;
        private readonly BulletView _bulletPrefab;
        private readonly PlayerConfig _config;
        private readonly ScreenWrapService _screenWrap;
        private readonly CustomObjectPool<Bullet> _bulletPool;
        private readonly List<Bullet> _activeBullets = new List<Bullet>();

        private float _lastFireTime;

        public BulletService(
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
                createFunc: () => new Bullet(_instantiator.InstantiatePrefabForComponent<BulletView>(_bulletPrefab), _config),
                actionOnGet: b => b.View.GameObject.SetActive(true),
                actionOnRelease: b => b.View.GameObject.SetActive(false)
            );
            
            _bulletPool = new CustomObjectPool<Bullet>(
                createFunc: () => new Bullet(_instantiator.InstantiatePrefabForComponent<BulletView>(_bulletPrefab), _config),
                actionOnGet: bullet => 
                {
                    bullet.View.GameObject.SetActive(true);
                    _activeBullets.Add(bullet);
                },
                actionOnRelease: b => 
                {
                    b.View.GameObject.SetActive(false);
                    _activeBullets.Remove(b);
                }
            );
        }

        public IReadOnlyList<Bullet> GetActiveBullets() => _activeBullets;

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

        private async UniTask DeactivateBulletAfterTimeAsync(Bullet bullet, CancellationToken token)
        {
            int delayMilliseconds = Mathf.RoundToInt(_config.BulletLifeTimeSeconds * SecondsToMilliseconds);
            bool isCancelled = await UniTask.Delay(delayMilliseconds, cancellationToken: token).SuppressCancellationThrow();

            if (!isCancelled) ReleaseBullet(bullet);
        }

        public void ReleaseBullet(Bullet bullet)
        {
            bullet.Cts?.Cancel();
            _bulletPool.Release(bullet);
        }

        public void Tick()
        {
            float dt = Time.deltaTime;
            
            foreach (var bullet in _activeBullets)
            {
                bullet.PhysicsBody.UpdateState(dt);
                _screenWrap.Wrap(bullet.PhysicsBody);
                bullet.View.Transform.position = bullet.PhysicsBody.Position;
                bullet.View.Transform.rotation = Quaternion.Euler(0, 0, bullet.PhysicsBody.Rotation);
            }
        }

        public void Dispose()
        {
            for (int i = _activeBullets.Count - 1; i >= 0; i--)
            {
                var bullet = _activeBullets[i];
                bullet.Cts?.Cancel();
                bullet.Cts?.Dispose();
            }
        }
    }
}