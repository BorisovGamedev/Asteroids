using System;

namespace Asteroids.Core
{
    public enum GameState
    {
        Bootstrap,
        MainMenu,
        Gameplay,
        GameOver
    }

    public class GameStateMachine
    {
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            
            OnStateChanged?.Invoke(CurrentState);

            switch (CurrentState)
            {
                case GameState.MainMenu:
                    UnityEngine.Debug.Log("Вход в Главное Меню");
                    break;
                
                case GameState.Gameplay:
                    UnityEngine.Debug.Log("Старт Игры! Спавн корабля и астероидов...");
                    // Позже здесь вызовем GameSystemFacade.StartGame()
                    break;
                
                case GameState.GameOver:
                    UnityEngine.Debug.Log("Игра окончена!");
                    break;
            }
        }
    }
}