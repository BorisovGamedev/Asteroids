using System;
using UnityEngine.SceneManagement;
using Zenject;

namespace Asteroids.Core
{
    public class GameSystemFacade : IInitializable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly SignalBus _signalBus;
        
        private readonly TickableManager _tickableManager;

        public GameSystemFacade(
            GameStateMachine stateMachine, 
            SignalBus signalBus,
            TickableManager tickableManager)
        {
            _stateMachine = stateMachine;
            _signalBus = signalBus;
            _tickableManager = tickableManager;
        }

        public void Initialize()
        {
            StartGame();

            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void StartGame()
        {
            _stateMachine.ChangeState(GameState.Gameplay);
            _tickableManager.IsPaused = false;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        }
        
        private void OnPlayerDied()
        {
            _tickableManager.IsPaused = true;
            
            _stateMachine.ChangeState(GameState.GameOver);
        }

        public void RestartGame()
        {
            _tickableManager.IsPaused = false;
            
            SceneManager.LoadScene("Game"); 
        }
    }
}