using TMPro;
using UnityEngine;
using Zenject;

namespace Asteroids.UI
{
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coordinatesTxt;
        [SerializeField] private TextMeshProUGUI _rotationTxt;
        [SerializeField] private TextMeshProUGUI _speedTxt;
        [SerializeField] private TextMeshProUGUI _laserTxt;
        [SerializeField] private TextMeshProUGUI _scoreTxt;
        [SerializeField] private TextMeshProUGUI _healthTxt;

        private HudViewModel _viewModel;

        [Inject]
        public void Construct(HudViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void Update()
        {
            if (_viewModel == null) return;

            _coordinatesTxt.text = _viewModel.CoordinatesText;
            _rotationTxt.text = _viewModel.RotationText;
            _speedTxt.text = _viewModel.SpeedText;
            _laserTxt.text = _viewModel.LaserText;
            _scoreTxt.text = _viewModel.ScoreText;
            _healthTxt.text = _viewModel.HealthText;
        }
    }
}