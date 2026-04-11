using UnityEngine;

namespace Asteroids.InputService
{
    public interface IInputService
    {
        float ForwardThrust { get; }
        float Rotation { get; }

        Vector2 DirectionVector { get; }
        bool IsVectorControl { get; }

        // Стрельба
        bool IsFiring { get; }
        bool IsFiringLaser { get; }
    }
}