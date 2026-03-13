using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities
{
    public class PlayerController : ITickable
    {
        private readonly CustomPhysicsBody _physicsBody;
        private readonly ScreenWrapService _screenWrap;
        private readonly Transform _playerViewTransform;
        private readonly PlayerConfig _config;

        public PlayerController(Transform playerViewTransform, PlayerConfig config, ScreenWrapService screenWrap)
        {
            _playerViewTransform = playerViewTransform;
            _config = config;
            _screenWrap = screenWrap;
            
            _physicsBody = new CustomPhysicsBody(Vector2.zero, 0f, _config.MaxSpeed, _config.Drag);
        }
        
        public void Tick()
        {
            HandleInput();
            
            _physicsBody.UpdateState(Time.deltaTime);
            
            _screenWrap.Warp(_physicsBody);
            
            _playerViewTransform.position = _physicsBody.Position;
            _playerViewTransform.rotation = Quaternion.Euler(0f, 0f, _physicsBody.Rotation);
        }

        private void HandleInput()
        {
            float turnInput = -UnityEngine.Input.GetAxis("Horizontal");

            if (turnInput != 0)
            {
                _physicsBody.Rotation += turnInput * _config.RotationSpeed * Time.deltaTime;
            }
            
            float forwardInput = UnityEngine.Input.GetAxis("Vertical");

            if (forwardInput > 0)
            {
                Vector2 thrust = _physicsBody.ForwardDirection * (_config.Acceleration * forwardInput);
                _physicsBody.AddForce(thrust, Time.deltaTime);
            }
        }
    }
}