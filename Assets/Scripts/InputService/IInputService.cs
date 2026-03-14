namespace Asteroids.InputService
{
    public interface IInputService
    {
        float ForwardThrust { get; }
        float Rotation { get; }
        bool IsFiring { get; }
        bool IsFiringLaser { get; }
    }
}