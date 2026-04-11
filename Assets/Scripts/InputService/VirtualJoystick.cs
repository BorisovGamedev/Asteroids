using UnityEngine;
using UnityEngine.EventSystems;

namespace Asteroids.InputService
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Настройки")]
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _handle;
        
        [SerializeField] private float _handleRange = 100f;

        public Vector2 InputVector { get; private set; }
        
        public bool IsPressed { get; private set; }

        private Vector2 _startPosition;

        private void Start()
        {
            _startPosition = _handle.anchoredPosition;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
            {
                Vector2 clampedPosition = Vector2.ClampMagnitude(localPoint, _handleRange);
                
                _handle.anchoredPosition = _startPosition + clampedPosition;

                InputVector = clampedPosition / _handleRange;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            InputVector = Vector2.zero;
            
            _handle.anchoredPosition = _startPosition;
        }
    }
}