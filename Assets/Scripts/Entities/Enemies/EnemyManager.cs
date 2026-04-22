using System.Collections.Generic;
using Asteroids.Configs;
using Asteroids.Core;
using UnityEngine;

namespace Asteroids.Entities.Enemies
{
    public class EnemyManager
    {
        private readonly EnemyFactory _factory;
        
        private readonly Dictionary<EnemyType, CustomObjectPool<IEnemy>> _pools;

        private readonly List<IEnemy> _activeEnemies = new List<IEnemy>();

        public EnemyManager(EnemyFactory factory)
        {
            _factory = factory;
            _pools = new Dictionary<EnemyType, CustomObjectPool<IEnemy>>();

            InitializePool(EnemyType.AsteroidBig);
            InitializePool(EnemyType.AsteroidSmall);
            InitializePool(EnemyType.Ufo);
        }

        private void InitializePool(EnemyType type)
        {
            _pools[type] = new CustomObjectPool<IEnemy>(
                createFunc: () => _factory.Create(type),
                actionOnGet: e => 
                {
                    e.GameObject.SetActive(true);
                    _activeEnemies.Add(e);
                },
                actionOnRelease: e => 
                {
                    e.GameObject.SetActive(false);
                    _activeEnemies.Remove(e);
                }
            );
        }

        public IReadOnlyList<IEnemy> GetActiveEnemies() => _activeEnemies;

        public IEnemy GetEnemy(EnemyType type)
        {
            return _pools[type].Get();
        }

        public void ReleaseEnemy(IEnemy enemy)
        {
            _pools[enemy.Type].Release(enemy);
        }
    }
}