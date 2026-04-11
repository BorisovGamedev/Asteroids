using UnityEngine;

namespace Asteroids.InputService
{
    public class MobileInputService : IInputService
    {
        private readonly VirtualJoystick _joystick;
        
        public bool IsFiringPressed;
        public bool IsLaserPressed;

        public MobileInputService(VirtualJoystick joystick)
        {
            _joystick = joystick;
        }

        public bool IsVectorControl => true;
        public Vector2 DirectionVector => _joystick.InputVector;

        public float ForwardThrust => _joystick.InputVector.magnitude > 0.1f ? 1f : 0f;

        public float Rotation => 0f;

        public bool IsFiring => IsFiringPressed;
        public bool IsFiringLaser => IsLaserPressed;

        public void SetFiring(bool isFiring) => IsFiringPressed = isFiring;
        public void SetLaser(bool isLaser) => IsLaserPressed = isLaser;
    }
}