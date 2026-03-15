using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    public class UfoWanderState : IUfoState
    {
        private readonly CustomPhysicsBody _ufoPhysics;
        private readonly float _speed;
        private Vector2 _wanderDirection;

        public UfoWanderState(CustomPhysicsBody ufoPhysics, float speed)
        {
            _ufoPhysics = ufoPhysics;
            _speed = speed;
        }

        public void Enter()
        {
            _wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            _ufoPhysics.SetVelocity(_wanderDirection * _speed);
        }

        public void Tick(float deltaTime)
        {
            _ufoPhysics.UpdateState(deltaTime);
        }

        public void Exit() { }
    }
}