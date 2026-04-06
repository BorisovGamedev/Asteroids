using UnityEngine;

namespace Asteroids.Physics
{
    public class CustomPhysicsBody
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; private set; }
        
        public float Rotation
        {
            get => _rotation;
            set => _rotation = PhysicsMath.NormalizeAngle(value);
        }
        
        public float MaxSpeed { get; set; }
        public float Drag { get; set; }
        
        public float Radius { get; set; }
        
        public Vector2 ForwardDirection => Quaternion.Euler(0, 0, Rotation) * Vector2.up;
        
        private float _rotation;

        public CustomPhysicsBody(Vector2 startPosition, float rotation, float maxSpeed, float drag, float radius = 0.5f)
        {
            Position = startPosition;
            Rotation = rotation;
            MaxSpeed = maxSpeed;
            Drag = drag;
            Radius = radius;
            Velocity = Vector2.zero;
        }

        public void AddForce(Vector2 force, float deltaTime)
        {
            Velocity += force * deltaTime;
        }
        
        public void SetVelocity(Vector2 newVelocity)
        {
            Velocity = newVelocity;
            
            if (Velocity.sqrMagnitude > MaxSpeed * MaxSpeed)
            {
                Velocity = Velocity.normalized * MaxSpeed;
            }
        }

        public void UpdateState(float deltaTime)
        {
            Velocity = Vector2.Lerp(Velocity, Vector2.zero, Drag * deltaTime);

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

        public bool IsCollidingWith(CustomPhysicsBody other)
        {
            float radiusSum = this.Radius + other.Radius;
            
            float sqrDistance = (this.Position - other.Position).sqrMagnitude;
            
            return sqrDistance <= (radiusSum * radiusSum);
        }

        public void BounceOff(CustomPhysicsBody other)
        {
            Vector2 pushDirection = (this.Position - other.Position).normalized;
            
            Velocity = pushDirection * (MaxSpeed * 0.8f);

            this.Position += pushDirection * 0.1f;
        }
    }
}