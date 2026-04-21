using UnityEngine;
using Asteroids.Configs;

namespace Asteroids.InputService
{
    public class MobileInputService : IInputService
    {
        private readonly VirtualJoystick _joystick;
        private readonly float _deadzone;
        
        private bool _isFiringPressed;
        private bool _isLaserPressed;
        
        public bool IsFiring => _isFiringPressed;
        public bool IsFiringLaser => _isLaserPressed;
        
        public void SetFiring(bool isFiring) => _isFiringPressed = isFiring;
        public void SetLaser(bool isLaser) => _isLaserPressed = isLaser;

        public MobileInputService(VirtualJoystick joystick, IConfigProvider configProvider)
        {
            _joystick = joystick;
            _deadzone = configProvider.Player.JoystickDeadzone;
        }

        public bool IsVectorControl => true;
        public Vector2 DirectionVector => _joystick.InputVector;

        public float ForwardThrust => _joystick.InputVector.magnitude > _deadzone ? 1f : 0f;

        public float Rotation => 0f;
    }
}