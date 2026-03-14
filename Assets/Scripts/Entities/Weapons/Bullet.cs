using System.Threading;
using Asteroids.Physics;

namespace Asteroids.Entities.Weapons
{
    public class Bullet
    {
        public BulletView View { get; }
        public CustomPhysicsBody PhysicsBody { get; }
        public CancellationTokenSource Cts { get; set; }

        public Bullet(BulletView view, float radius )
        {
            View = view;
            
            PhysicsBody = new CustomPhysicsBody(UnityEngine.Vector2.zero, 0f, 7f, drag: 0f, radius: radius);
        }
    }
}