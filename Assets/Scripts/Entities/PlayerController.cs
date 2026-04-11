using System;
using System.Threading;
using Asteroids.Configs;
using Asteroids.Core;
using Asteroids.Entities.Weapons;
using Asteroids.InputService;
using Asteroids.Physics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities
{
    public class PlayerController : ITickable
    {
        public CustomPhysicsBody PhysicsBody { get; }
        
        public int CurrentHealth { get; private set; }
        public bool IsInvulnerable { get; private set; }
        public bool IsDead => CurrentHealth <= 0;

        private readonly PlayerView _view;
        private readonly IConfigProvider _configProvider;
        private readonly ScreenWrapService _screenWrap;
        private readonly IInputService _input;
        private readonly WeaponService _weaponService;
        private readonly PlayerConfig _config;
        private readonly SignalBus _signalBus;

        private CancellationTokenSource _invulnerabilityCts;

        public PlayerController(
            PlayerView view,
            IConfigProvider configProvider,
            ScreenWrapService screenWrap,
            IInputService input,
            WeaponService weaponService,
            SignalBus signalBus)
        {
            _view = view;
            _configProvider = configProvider;
            _screenWrap = screenWrap;
            _input = input;
            _weaponService = weaponService;
            _signalBus = signalBus;
            
            _config = _configProvider.Player;

            PhysicsBody = new CustomPhysicsBody(Vector2.zero, 0f, _config.MaxSpeed, _config.Drag, radius: _config.PlayerRadius);
            _view.DebugRadius = PhysicsBody.Radius;

            CurrentHealth = _config.MaxHealth;
            IsInvulnerable = false;
        }

        public void TakeDamage(Vector2 enemyPosition)
        {
            if (IsInvulnerable || IsDead) return;

            CurrentHealth--;
            Debug.Log($"Игрок получил урон! Осталось ХП: {CurrentHealth}");

            Vector2 pushDirection = (PhysicsBody.Position - enemyPosition).normalized;
            PhysicsBody.SetVelocity(pushDirection * _config.KnockbackForce);

            if (CurrentHealth > 0)
            {
                StartInvulnerabilityAsync().Forget();
            }
            else
            {
                _view.GameObject.SetActive(false);
                _signalBus.Fire<PlayerDiedSignal>(); 
            }
        }

        private async UniTaskVoid StartInvulnerabilityAsync()
        {
            IsInvulnerable = true;
            
            _view.ShieldParticles.gameObject.SetActive(true);
            _view.ShieldParticles.Play();

            _invulnerabilityCts?.Cancel();
            _invulnerabilityCts?.Dispose();
            _invulnerabilityCts = new CancellationTokenSource();

            int delayMs = Mathf.RoundToInt(_config.InvulnerabilitySeconds * 1000f);

            bool isCancelled = await UniTask.Delay(delayMs, cancellationToken: _invulnerabilityCts.Token).SuppressCancellationThrow();

            if (!isCancelled && !IsDead)
            {
                IsInvulnerable = false;
                
                _view.ShieldParticles.Stop();
                _view.ShieldParticles.gameObject.SetActive(false);
            }
        }

        public void Tick()
        {
            if (IsDead) return;

            if (!IsInvulnerable)
            {
                HandleInput();
            }

            PhysicsBody.UpdateState(Time.deltaTime);
            _screenWrap.Wrap(PhysicsBody);

            _view.Transform.position = PhysicsBody.Position;
            _view.Transform.rotation = Quaternion.Euler(0, 0, PhysicsBody.Rotation);
        }

        private void HandleInput()
        {
            if (_input.IsVectorControl)
            {
                Vector2 inputDir = _input.DirectionVector;

                if (inputDir.sqrMagnitude > 0.01f)
                {
                    float targetAngle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg - 90f;

                    PhysicsBody.Rotation = Mathf.MoveTowardsAngle(
                        PhysicsBody.Rotation, 
                        targetAngle, 
                        _config.RotationSpeed * Time.deltaTime);

                    Vector2 thrust = PhysicsBody.ForwardDirection * (_config.Acceleration * inputDir.magnitude);
                    PhysicsBody.AddForce(thrust, Time.deltaTime);
                }
            }
            else
            {
                float turnInput = _input.Rotation;
                if (turnInput != 0)
                {
                    PhysicsBody.Rotation += -turnInput * _config.RotationSpeed * Time.deltaTime;
                }

                float forwardInput = _input.ForwardThrust;
                if (forwardInput > 0)
                {
                    Vector2 thrust = PhysicsBody.ForwardDirection * (_config.Acceleration * forwardInput);
                    PhysicsBody.AddForce(thrust, Time.deltaTime);
                }
            }

            if (_input.IsFiring)
            {
                _weaponService.Fire(PhysicsBody.Position, PhysicsBody.Rotation, PhysicsBody.ForwardDirection);
            }

            if (_input.IsFiringLaser)
            {
                _weaponService.FireLaser(PhysicsBody.Position, PhysicsBody.ForwardDirection);
            }
        }
    }
}