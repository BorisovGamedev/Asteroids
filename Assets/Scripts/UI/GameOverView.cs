using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Asteroids.UI
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _menuBtn;

        private GameOverViewModel _viewModel;

        [Inject]
        public void Construct(GameOverViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Start()
        {
            _panel.SetActive(false);
            
            _viewModel.OnGameOverStateChanged += SetVisible;
            
            _restartBtn.onClick.AddListener(() => _viewModel.RestartClicked());
            _menuBtn.onClick.AddListener(() => _viewModel.MenuClicked());
        }

        private void OnDestroy()
        {
            if (_viewModel != null)
            {
                _viewModel.OnGameOverStateChanged -= SetVisible;
            }
        }

        private void SetVisible(bool isVisible)
        {
            _panel.SetActive(isVisible);
        }
    }
}