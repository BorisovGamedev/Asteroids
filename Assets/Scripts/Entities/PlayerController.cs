using Asteroids.Configs;
using Asteroids.Physics;
using Asteroids.InputService;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities
{
    public class PlayerController : ITickable
    {
        private readonly CustomPhysicsBody _physicsBody;
        private readonly ScreenWrapService _screenWrap;
        private readonly PlayerView _view;
        private readonly PlayerConfig _config;
        private readonly IInputService _input;

        public PlayerController(
            PlayerView view, 
            IConfigProvider configProvider, 
            ScreenWrapService screenWrap,
            IInputService input)
        {
            _view = view;
            _config = configProvider.Player;
            _screenWrap = screenWrap;
            _input = input;
            
            _physicsBody = new CustomPhysicsBody(Vector2.zero, 0f, _config.MaxSpeed, _config.Drag);
        }
        
        public void Tick()
        {
            HandleInput();
            
            _physicsBody.UpdateState(Time.deltaTime);
            _screenWrap.Warp(_physicsBody);
            
            _view.Transform.position = _physicsBody.Position;
            _view.Transform.rotation = Quaternion.Euler(0f, 0f, _physicsBody.Rotation);
        }

        private void HandleInput()
        {
            float turnInput = _input.Rotation;

            if (turnInput != 0)
            {
                _physicsBody.Rotation += -turnInput * _config.RotationSpeed * Time.deltaTime;
            }
            
            float forwardInput = _input.ForwardThrust;

            if (forwardInput > 0)
            {
                Vector2 thrust = _physicsBody.ForwardDirection * (_config.Acceleration * forwardInput);
                _physicsBody.AddForce(thrust, Time.deltaTime);
            }
        }
    }
}