using TMPro;
using UnityEngine;
using Zenject;

namespace Asteroids.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coordinatesText;
        [SerializeField] private TextMeshProUGUI _rotationText;
        [SerializeField] private TextMeshProUGUI _speedText;
        [SerializeField] private TextMeshProUGUI _laserText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _healthText;

        private HudViewModel _viewModel;

        [Inject]
        public void Construct(HudViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Update()
        {
            if (_viewModel == null) return;

            _coordinatesText.text = _viewModel.CoordinatesText;
            _rotationText.text = _viewModel.RotationText;
            _speedText.text = _viewModel.SpeedText;
            _laserText.text = _viewModel.LaserText;
            _scoreText.text = _viewModel.ScoreText;
            _healthText.text = _viewModel.HealthText;
        }
    }
}