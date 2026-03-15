using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    public class Asteroid : IEnemy
    {
        public EnemyType Type => Size == 2 ? EnemyType.AsteroidBig : EnemyType.AsteroidSmall;
        public CustomPhysicsBody PhysicsBody { get; }
        public Transform ViewTransform => _view.Transform;
        public GameObject GameObject => _view.GameObject;

        private readonly AsteroidView _view;
        public int Size { get; private set; } 
        
        public Asteroid(AsteroidView view, EnemiesConfig config)
        {
            _view = view;

            PhysicsBody = new CustomPhysicsBody(
                startPosition: Vector2.zero, 
                rotation: 0f, 
                maxSpeed: config.AsteroidMaxSpeed,
                drag: 0f,
                radius: config.BigAsteroidRadius);
        }

        public void Launch(Vector2 position, Vector2 direction, float speed, int size, EnemiesConfig config)
        {
            Size = size;
            
            PhysicsBody.Radius = size == 2 ? config.BigAsteroidRadius : config.SmallAsteroidRadius;
            float scale = size == 2 ? config.BigAsteroidScale : config.SmallAsteroidScale;
            
            _view.Transform.localScale = new Vector3(scale, scale, 1f);
            
            PhysicsBody.Position = position;
            PhysicsBody.Stop();
            
            PhysicsBody.AddForce(direction * speed, 1f);
            
            _view.DebugRadius = PhysicsBody.Radius;
        }
        
        public void Tick(float deltaTime)
        {
            PhysicsBody.UpdateState(deltaTime);
            ViewTransform.position = PhysicsBody.Position;
            ViewTransform.Rotate(Vector3.forward, 45f * deltaTime);
        }
    }
}