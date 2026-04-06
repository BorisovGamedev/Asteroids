using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    public class UfoChaseState : IUfoState
    {
        private readonly UfoEnemy _ufo;
        private readonly PlayerController _player;
        private readonly float _speed;

        public UfoChaseState(UfoEnemy ufo, PlayerController player, float speed)
        {
            _ufo = ufo;
            _player = player;
            _speed = speed;
        }

        public void Enter() { }

        public void Tick(float deltaTime)
        {
            if (_player.IsDead || _player.IsInvulnerable)
            {
                _ufo.ChangeState(_ufo.WanderState);
                return;
            }

            Vector2 directionToPlayer = (_player.PhysicsBody.Position - _ufo.PhysicsBody.Position).normalized;
            
            _ufo.PhysicsBody.SetVelocity(directionToPlayer * _speed);
            _ufo.PhysicsBody.UpdateState(deltaTime);
        }

        public void Exit() { }
    }
}