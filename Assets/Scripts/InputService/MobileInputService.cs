using UnityEngine;
using Asteroids.Configs;

namespace Asteroids.InputService
{
    public class MobileInputService : IInputService
    {
        private readonly VirtualJoystick _joystick;
        private readonly float _deadzone;
        
        public bool IsFiringPressed;
        public bool IsLaserPressed;

        public MobileInputService(VirtualJoystick joystick, IConfigProvider configProvider)
        {
            _joystick = joystick;
            _deadzone = configProvider.Player.JoystickDeadzone;
        }

        public bool IsVectorControl => true;
        public Vector2 DirectionVector => _joystick.InputVector;

        public float ForwardThrust => _joystick.InputVector.magnitude > _deadzone ? 1f : 0f;

        public float Rotation => 0f;

        public bool IsFiring => IsFiringPressed;
        public bool IsFiringLaser => IsLaserPressed;

        public void SetFiring(bool isFiring) => IsFiringPressed = isFiring;
        public void SetLaser(bool isLaser) => IsLaserPressed = isLaser;
    }
}