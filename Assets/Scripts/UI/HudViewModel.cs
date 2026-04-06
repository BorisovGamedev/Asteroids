using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Entities;
using Asteroids.Entities.Weapons;
using UnityEngine;
using Zenject;

namespace Asteroids.UI
{
    public class HudViewModel : ITickable
    {
        private readonly PlayerController _player;
        private readonly WeaponService _weapon;
        private readonly ScoreManager _score;
        private readonly PlayerConfig _config;

        public string CoordinatesText { get; private set; }
        public string RotationText { get; private set; }
        public string SpeedText { get; private set; }
        public string LaserText { get; private set; }
        public string ScoreText { get; private set; }
        public string HealthText { get; private set; }

        public HudViewModel(PlayerController player, WeaponService weapon, ScoreManager score, IConfigProvider configProvider)
        {
            _player = player;
            _weapon = weapon;
            _score = score;
            _config = configProvider.Player;
        }

        public void Tick()
        {
            Vector2 pos = _player.PhysicsBody.Position;
            CoordinatesText = $"POS: {pos.x:F1} : {pos.y:F1}";

            RotationText = $"ROT: {Mathf.RoundToInt(_player.PhysicsBody.Rotation)}°";

            float speed = _player.PhysicsBody.Velocity.magnitude;
            SpeedText = $"SPD: {Mathf.RoundToInt(speed)}";

            if (_weapon.CurrentLaserCharges >= _config.MaxLaserCharges)
            {
                LaserText = $"LASER: {_weapon.CurrentLaserCharges} (READY)";
            }
            else
            {
                float cooldownLeft = _config.LaserCooldownSeconds - _weapon.LaserCooldownTimer;
                LaserText = $"LASER: {_weapon.CurrentLaserCharges} ({cooldownLeft:F1}s)";
            }

            ScoreText = $"SCORE: {_score.CurrentScore}";
            
            HealthText = $"HP: {new string('♥', _player.CurrentHealth)}";
        }
    }
}