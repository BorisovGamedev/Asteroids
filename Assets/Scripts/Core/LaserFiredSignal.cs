using UnityEngine;
using Asteroids.Configs;

namespace Asteroids.Core
{
    public class LaserFiredSignal
    {
        public Vector2 Origin { get; }
        public Vector2 Direction { get; }
        public float Length { get; }

        public LaserFiredSignal(Vector2 origin, Vector2 direction, float length)
        {
            Origin = origin;
            Direction = direction;
            Length = length;
        }
    }
    
    public class EnemyKilledSignal
    {
        public EnemyType Type { get; }
            
        public EnemyKilledSignal(EnemyType type) => Type = type;
    }
    
    public class PlayerDiedSignal { }
}