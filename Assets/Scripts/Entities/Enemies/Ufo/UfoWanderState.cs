using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    public class UfoWanderState : IUfoState
    {
        private readonly UfoEnemy _ufo;
        private readonly PlayerController _player;
        private readonly float _speed;
        private readonly float _agroRadiusSqr;
        
        private Vector2 _wanderDirection;

        public UfoWanderState(UfoEnemy ufo, PlayerController player, float speed, float agroRadius)
        {
            _ufo = ufo;
            _player = player;
            _speed = speed;
            _agroRadiusSqr = agroRadius * agroRadius;
        }

        public void Enter()
        {
            _wanderDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            _ufo.PhysicsBody.SetVelocity(_wanderDirection * _speed);
        }

        public void Tick(float deltaTime)
        {
            _ufo.PhysicsBody.UpdateState(deltaTime);

            if (!_player.IsDead && !_player.IsInvulnerable)
            {
                // Считаем квадрат расстояния до игрока
                float distanceSqr = (_player.PhysicsBody.Position - _ufo.PhysicsBody.Position).sqrMagnitude;
                
                if (distanceSqr <= _agroRadiusSqr)
                {
                    _ufo.ChangeState(_ufo.ChaseState);
                }
            }
        }

        public void Exit() { }
    }
}