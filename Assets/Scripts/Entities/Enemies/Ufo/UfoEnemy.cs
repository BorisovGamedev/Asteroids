using Asteroids.Configs;
using Asteroids.Physics;
using UnityEngine;

namespace Asteroids.Entities.Enemies.Ufo
{
    public class UfoEnemy : IEnemy
    {
        public EnemyType Type => EnemyType.Ufo;
        public CustomPhysicsBody PhysicsBody { get; }
        public Transform ViewTransform => _view.Transform;
        public GameObject GameObject => _view.GameObject;

        private readonly UfoView _view;
        private IUfoState _currentState;

        public UfoChaseState ChaseState { get; }
        public UfoWanderState WanderState { get; }

        public UfoEnemy(UfoView view, PlayerController player, EnemiesConfig config)
        {
            _view = view;
            
            PhysicsBody = new CustomPhysicsBody(Vector2.zero, 0f, config.UfoSpeed, drag: 0f, radius: config.UfoRadius);
            _view.Transform.localScale = new Vector3(config.UfoScale, config.UfoScale, 1f);

            ChaseState = new UfoChaseState(this, player, config.UfoSpeed);
            WanderState = new UfoWanderState(this, player, config.UfoSpeed, config.UfoAgroRadius); // Передаем радиус агро!
            
            _view.DebugRadius = PhysicsBody.Radius;
        }

        public void Launch(Vector2 position)
        {
            PhysicsBody.Position = position;
            PhysicsBody.Stop();
            
            ChangeState(WanderState);
        }

        public void ChangeState(IUfoState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            _currentState?.Tick(deltaTime);
            ViewTransform.position = PhysicsBody.Position;
        }
    }
}