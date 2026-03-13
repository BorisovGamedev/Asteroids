using UnityEngine;

namespace Asteroids.Physics
{
    public class CustomPhysicsBody
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; private set; }
        public float Rotation { get; set; }
        
        public float MaxSpeed { get; set; }
        public float Drag { get; set; }
        
        public Vector2 ForwardDirection => Quaternion.Euler(0, 0, Rotation) * Vector2.up;

        public CustomPhysicsBody(Vector2 startPosition, float rotation, float maxSpeed, float drag)
        {
            Position = startPosition;
            Rotation = rotation;
            MaxSpeed = maxSpeed;
            Drag = drag;
            Velocity = Vector2.zero;
        }

        public void AddForce(Vector2 force, float deltaTime)
        {
            Velocity += force * deltaTime;
        }

        public void UpdateState(float deltaTime)
        {
            Velocity = Velocity.normalized * MaxSpeed;

            if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            {
                Velocity = Velocity.normalized * MaxSpeed;
            }
            
            Position += Velocity * deltaTime;
        }
        
        public void Stop()
        {
            Velocity = Vector2.zero;
        }
    }
}