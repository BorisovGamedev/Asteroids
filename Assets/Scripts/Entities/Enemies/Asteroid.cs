using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    public class Asteroid : IEnemy
    {
        public EnemyType Type { get; private set; }
        
        public CustomPhysicsBody PhysicsBody { get; }
        public Transform ViewTransform => _view.Transform;
        public GameObject GameObject => _view.GameObject;

        private readonly AsteroidView _view;

        public Asteroid(AsteroidView view, EnemiesConfig config)
        {
            _view = view;
            PhysicsBody = new CustomPhysicsBody(Vector2.zero, 0f, config.AsteroidMaxSpeed, drag: 0f, radius: config.BigAsteroidRadius);
        }

        public void Launch(Vector2 position, Vector2 direction, float speed, EnemyType type, EnemiesConfig config)
        {
            Type = type;
            
            bool isBig = type == EnemyType.AsteroidBig;
            PhysicsBody.Radius = isBig ? config.BigAsteroidRadius : config.SmallAsteroidRadius;
            float scale = isBig ? config.BigAsteroidScale : config.SmallAsteroidScale;
            
            _view.Transform.localScale = new Vector3(scale, scale, 1f);
            
            _view.DebugRadius = PhysicsBody.Radius;

            PhysicsBody.Position = position;
            PhysicsBody.Stop(); 
            PhysicsBody.AddForce(direction * speed, 1f); 
        }

        public void Tick(float deltaTime)
        {
            PhysicsBody.UpdateState(deltaTime);
            ViewTransform.position = PhysicsBody.Position;
            ViewTransform.Rotate(Vector3.forward, 45f * deltaTime);
        }
    }
}