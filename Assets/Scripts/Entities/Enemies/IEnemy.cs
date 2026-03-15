using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    public enum EnemyType
    {
        AsteroidBig,
        AsteroidSmall,
        Ufo
    }
    
    public interface IEnemy
    {
        EnemyType Type { get; }
        CustomPhysicsBody PhysicsBody { get; }
        Transform ViewTransform { get; }
        GameObject GameObject { get; }
        
        void Tick(float deltaTime);
    }
}