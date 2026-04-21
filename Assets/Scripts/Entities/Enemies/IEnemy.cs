using Asteroids.Physics;
using Asteroids.Configs;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    
    public interface IEnemy
    {
        EnemyType Type { get; }
        CustomPhysicsBody PhysicsBody { get; }
        Transform ViewTransform { get; }
        GameObject GameObject { get; }
        
        void Tick(float deltaTime);
    }
}