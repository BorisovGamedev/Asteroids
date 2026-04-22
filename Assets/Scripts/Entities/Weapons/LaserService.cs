using System;
using Asteroids.Core;
using Asteroids.Configs;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Weapons
{
    public class LaserService : ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly PlayerConfig _config;

        public int CurrentLaserCharges { get; private set; }
        public float LaserCooldownTimer { get; private set; }

        public event Action<Vector2, Vector2, float, int> OnLaserFiredVisual;

        public LaserService(SignalBus signalBus, IConfigProvider configProvider)
        {
            _signalBus = signalBus;
            _config = configProvider.Player;

            CurrentLaserCharges = _config.MaxLaserCharges;
            LaserCooldownTimer = 0f;
        }

        public void Tick()
        {
            if (CurrentLaserCharges < _config.MaxLaserCharges)
            {
                LaserCooldownTimer += Time.deltaTime;
                if (LaserCooldownTimer >= _config.LaserCooldownSeconds)
                {
                    CurrentLaserCharges++;
                    LaserCooldownTimer = 0f;
                }
            }
        }

        public void FireLaser(Vector2 origin, Vector2 direction)
        {
            if (CurrentLaserCharges <= 0) return;

            CurrentLaserCharges--;
            LaserCooldownTimer = 0f;

            float spawnOffset = _config.PlayerRadius + 0.1f;
            Vector2 startPosition = origin + (direction * spawnOffset);

            _signalBus.Fire(new LaserFiredSignal(startPosition, direction, _config.LaserLength));

            OnLaserFiredVisual?.Invoke(startPosition, direction, _config.LaserLength, _config.LaserVisualDurationMs);
        }
    }
}