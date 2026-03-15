using System.Threading;
using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Weapons
{
    public class Bullet
    {
        public BulletView View { get; }
        public CustomPhysicsBody PhysicsBody { get; }
        public CancellationTokenSource Cts { get; set; }

        public Bullet(BulletView view, PlayerConfig config)
        {
            View = view;
            
            PhysicsBody = new CustomPhysicsBody(
                startPosition: Vector2.zero, 
                rotation: 0f, 
                maxSpeed: config.BulletSpeed,
                drag: 0f,
                radius: config.BulletRadius);
                
            View.DebugRadius = PhysicsBody.Radius;
        }
    }
}