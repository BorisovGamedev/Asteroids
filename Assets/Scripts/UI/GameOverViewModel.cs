using System;
using Asteroids.Core;
using Zenject;

namespace Asteroids.UI
{
    public class GameOverViewModel : IInitializable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly GameSystemFacade _facade;
        
        public event Action<bool> OnGameOverStateChanged;

        public GameOverViewModel(GameStateMachine stateMachine, GameSystemFacade facade)
        {
            _stateMachine = stateMachine;
            _facade = facade;
        }

        public void Initialize()
        {
            _stateMachine.OnStateChanged += HandleStateChanged;
        }

        public void Dispose()
        {
            _stateMachine.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            OnGameOverStateChanged?.Invoke(state == GameState.GameOver);
        }

        public void RestartClicked() => _facade.RestartGame();
        public void MenuClicked() => _facade.QuitToMenu();
    }
}