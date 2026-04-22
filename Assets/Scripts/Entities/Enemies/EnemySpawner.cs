using Asteroids.Configs;
using UnityEngine;
using Zenject;

namespace Asteroids.Entities.Enemies
{
    public class EnemySpawner : ITickable
    {
        private readonly EnemiesConfig _enemiesConfig;
        private readonly WorldConfig _worldConfig;
        private readonly EnemyManager _enemyManager;
        
        private float _spawnTimer;

        public EnemySpawner(IConfigProvider configProvider, EnemyManager enemyManager)
        {
            _enemiesConfig = configProvider.Enemies;
            _worldConfig = configProvider.World;
            _enemyManager = enemyManager;
        }

        public void Tick()
        {
            _spawnTimer += Time.deltaTime;
            
            if (_spawnTimer >= _worldConfig.SpawnDelaySeconds && _enemyManager.GetActiveEnemies().Count < _worldConfig.MaxEnemiesOnScreen)
            {
                _spawnTimer = 0f;
                
                if (Random.value < _worldConfig.UfoSpawnChance) SpawnUfo();
                else SpawnAsteroid(EnemyType.AsteroidBig);
            }
        }

        public void SpawnAsteroid(EnemyType type, Vector2? specificPosition = null)
        {
            IEnemy enemy = _enemyManager.GetEnemy(type);
            
            Vector2 spawnPos = specificPosition ?? GetRandomPositionOnEdge();
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            
            float baseSpeed = Random.Range(_enemiesConfig.AsteroidMinSpeed, _enemiesConfig.AsteroidMaxSpeed);
            float finalSpeed = type == EnemyType.AsteroidBig 
                ? baseSpeed 
                : baseSpeed * _enemiesConfig.AsteroidFragmentSpeedMultiplier;

            ((Asteroid)enemy).Launch(spawnPos, randomDirection, finalSpeed, type, _enemiesConfig);
        }

        private void SpawnUfo()
        {
            IEnemy enemy = _enemyManager.GetEnemy(EnemyType.Ufo);
            ((Ufo.UfoEnemy)enemy).Launch(GetRandomPositionOnEdge());
        }

        public void HandleAsteroidDestruction(IEnemy enemy)
        {
            if (enemy.Type == EnemyType.AsteroidBig)
            {
                for (int i = 0; i < _enemiesConfig.AsteroidFragmentsCount; i++)
                {
                    SpawnAsteroid(EnemyType.AsteroidSmall, enemy.PhysicsBody.Position);
                }
            }
        }

        private Vector2 GetRandomPositionOnEdge()
        {
            float w = _worldConfig.WorldWidth / 2f;
            float h = _worldConfig.WorldHeight / 2f;

            if (Random.value > 0.5f) return new Vector2(Random.value > 0.5f ? w : -w, Random.Range(-h, h));
            return new Vector2(Random.Range(-w, w), Random.value > 0.5f ? h : -h);
        }
    }
}