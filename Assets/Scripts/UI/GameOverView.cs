using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Asteroids.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _finalScoreTxt; 
        [SerializeField] private TextMeshProUGUI _leaderboardTxt; 
        
        [Header("Input Group")]
        [SerializeField] private GameObject _inputGroup; 
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _saveBtn;

        [Header("Standard Buttons")]
        [SerializeField] private Button _restartBtn;

        private GameOverViewModel _viewModel;

        [Inject]
        public void Construct(GameOverViewModel viewModel) => _viewModel = viewModel;

        private void Start()
        {
            _panel.SetActive(false);
            _viewModel.OnGameOverStateChanged += SetupPanel;
            
            _restartBtn.onClick.AddListener(() => _viewModel.RestartClicked());
            
            _saveBtn.onClick.AddListener(() => _viewModel.SaveScore(_nameInput.text));
        }

        private void OnDestroy()
        {
            if (_viewModel != null) _viewModel.OnGameOverStateChanged -= SetupPanel;
        }

        private void SetupPanel(bool isVisible, bool isNewHighScore, string finalScoreText, string leaderboardText)
        {
            _panel.SetActive(isVisible);
            if (!isVisible) return;

            _finalScoreTxt.text = finalScoreText;
            _leaderboardTxt.text = leaderboardText;

            if (isNewHighScore)
            {
                _inputGroup.SetActive(true);
            }
            else
            {
                _inputGroup.SetActive(false);
                _nameInput.text = "";
            }
        }
    }
}