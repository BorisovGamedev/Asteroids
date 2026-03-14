using Asteroids.Configs;
using Asteroids.Entities.Weapons;
using Asteroids.Physics;
using Asteroids.InputService;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities
{
    public class PlayerController : ITickable
    {
        public CustomPhysicsBody PhysicsBody { get; }
        
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerView _view;
        private readonly PlayerConfig _config;
        private readonly IInputService _input;
        private readonly WeaponService _weaponService;
        
        public PlayerController(
            PlayerView view, 
            IConfigProvider configProvider, 
            ScreenWrapService screenWrap,
            IInputService input,
            WeaponService weaponService)
        {
            _view = view;
            _config = configProvider.Player;
            _screenWrap = screenWrap;
            _input = input;
            _weaponService = weaponService;
            
            PhysicsBody = new CustomPhysicsBody(Vector2.zero, 0f, _config.MaxSpeed, _config.Drag, radius: _config.PlayerRadius);
        }
        
        public void Tick()
        {
            HandleInput();
            
            PhysicsBody.UpdateState(Time.deltaTime);
            _screenWrap.Wrap(PhysicsBody);
            
            _view.Transform.position = PhysicsBody.Position;
            _view.Transform.rotation = Quaternion.Euler(0f, 0f, PhysicsBody.Rotation);
        }

        private void HandleInput()
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
            
            if (_input.IsFiring)
            {
                _weaponService.Fire(PhysicsBody.Position, PhysicsBody.Rotation, PhysicsBody.ForwardDirection);
            }
        }
    }
}