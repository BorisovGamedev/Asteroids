using Asteroids.Physics;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Enemies
{
    public class EnemyMovementService : ITickable
    {
        private readonly EnemyManager _enemyManager;
        private readonly ScreenWrapService _screenWrap;

        public EnemyMovementService(EnemyManager enemyManager, ScreenWrapService screenWrap)
        {
            _enemyManager = enemyManager;
            _screenWrap = screenWrap;
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            
            foreach (var enemy in _enemyManager.GetActiveEnemies())
            {
                enemy.Tick(deltaTime);
                _screenWrap.Wrap(enemy.PhysicsBody);
            }
        }
    }
}