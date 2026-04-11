using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Asteroids.InputService
{
    public class MobileInputCatcher : MonoBehaviour
    {
        [SerializeField] private EventTrigger _fireButtonTrigger;
        [SerializeField] private EventTrigger _laserButtonTrigger;

        private MobileInputService _mobileInput;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _mobileInput = inputService as MobileInputService;
            
            if (_mobileInput == null)
            {
                gameObject.SetActive(false);
                return;
            }

            SetupTriggers();
        }

        private void SetupTriggers()
        {
            AddPointerEvents(_fireButtonTrigger, 
                onDown: () => _mobileInput.SetFiring(true), 
                onUp: () => _mobileInput.SetFiring(false));

            AddPointerEvents(_laserButtonTrigger, 
                onDown: () => _mobileInput.SetLaser(true), 
                onUp: () => _mobileInput.SetLaser(false));
        }

        private void AddPointerEvents(EventTrigger trigger, System.Action onDown, System.Action onUp)
        {
            var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            downEntry.callback.AddListener((_) => onDown?.Invoke());
            trigger.triggers.Add(downEntry);

            var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            upEntry.callback.AddListener((_) => onUp?.Invoke());
            trigger.triggers.Add(upEntry);
        }
    }
}