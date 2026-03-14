using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    public class Asteroid
    {
        public AsteroidView View { get; }
        public CustomPhysicsBody PhysicsBody { get; }
        public int Size { get; private set; }

        public Asteroid(AsteroidView view)
        {
            View = view;

            PhysicsBody = new CustomPhysicsBody(Vector2.zero, 0f, maxSpeed: 4f, drag: 0f, radius: 0.5f);
        }

        public void Launch(Vector2 position, Vector2 direction, float speed, int size, EnemiesConfig config)
        {
            Size = size;
            
            PhysicsBody.Radius = size == 2 ? config.BigAsteroidRadius : config.SmallAsteroidRadius;
            float scale = size == 2 ? config.BigAsteroidScale : config.SmallAsteroidScale;
            
            View.Transform.localScale = new Vector3(scale, scale, 1f);
            
            PhysicsBody.Position = position;
            PhysicsBody.Stop();
            
            PhysicsBody.AddForce(direction * speed, 1f);
        }
    }
}