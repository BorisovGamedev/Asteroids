using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    public class UfoChaseState : IUfoState
    {
        private readonly CustomPhysicsBody _ufoPhysics;
        private readonly CustomPhysicsBody _playerPhysics;
        private readonly float _speed;

        public UfoChaseState(CustomPhysicsBody ufoPhysics, CustomPhysicsBody playerPhysics, float speed)
        {
            _ufoPhysics = ufoPhysics;
            _playerPhysics = playerPhysics;
            _speed = speed;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            Vector2 directionToPlayer = (_playerPhysics.Position - _ufoPhysics.Position).normalized;
            
            _ufoPhysics.SetVelocity(directionToPlayer * _speed);
            
            _ufoPhysics.UpdateState(deltaTime);
        }

        public void Exit() { }
    }
}