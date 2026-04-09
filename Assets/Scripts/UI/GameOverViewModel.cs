using System;
using System.Text;
using Asteroids.Core;
using Zenject;

namespace Asteroids.UI
{
    public class GameOverViewModel : IInitializable, IDisposable
    {
        private readonly GameStateMachine _stateMachine;
        private readonly GameSystemFacade _facade;
        private readonly LeaderboardService _leaderboard;
        private readonly ScoreManager _scoreManager;

        private bool _isScoreSaved;

        public event Action<bool, bool, string, string> OnGameOverStateChanged;

        public GameOverViewModel(
            GameStateMachine stateMachine, 
            GameSystemFacade facade,
            LeaderboardService leaderboard,
            ScoreManager scoreManager)
        {
            _stateMachine = stateMachine;
            _facade = facade;
            _leaderboard = leaderboard;
            _scoreManager = scoreManager;
        }

        public void Initialize() => _stateMachine.OnStateChanged += HandleStateChanged;
        public void Dispose() => _stateMachine.OnStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.GameOver)
            {
                _isScoreSaved = false; 
                UpdateUI();
            }
            else
            {
                OnGameOverStateChanged?.Invoke(false, false, "", "");
            }
        }

        private void UpdateUI()
        {
            int score = _scoreManager.CurrentScore;
            
            bool isHighScore = !_isScoreSaved && _leaderboard.IsNewHighScore(score);
            
            string finalScoreText = $"Result: {score}";
            string leaderboardText = GetLeaderboardText();

            OnGameOverStateChanged?.Invoke(true, isHighScore, finalScoreText, leaderboardText);
        }

        public void SaveScore(string playerName)
        {
            if (_isScoreSaved) return;

            if (string.IsNullOrEmpty(playerName)) playerName = "Anonymous";
            
            _leaderboard.AddEntry(playerName, _scoreManager.CurrentScore);
            
            _isScoreSaved = true;
            
            UpdateUI(); 
        }

        public void RestartClicked() => _facade.RestartGame();
        public void MenuClicked() => _facade.QuitToMenu();

        private string GetLeaderboardText()
        {
            if (_leaderboard.Data.Entries.Count == 0) return "No records yet.";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _leaderboard.Data.Entries.Count; i++)
            {
                var entry = _leaderboard.Data.Entries[i];
                sb.AppendLine($"{i + 1}. {entry.PlayerName} - {entry.Score}");
            }
            return sb.ToString();
        }
    }
}