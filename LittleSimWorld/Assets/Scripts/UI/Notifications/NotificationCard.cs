using GUI_Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Notifications
{
    [RequireComponent(typeof(SlidingPopup))]
    public class NotificationCard : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Transform hidePosition = null;
        [SerializeField] private TextMeshProUGUI notifMessage = null;
        [SerializeField] private Image notifIcon = null;
        [SerializeField] private float dockScaleRatio = 1f;
        [SerializeField] private TextMeshProUGUI timeStamp = null;
        
        private const float HidingAlpha = 0.5f;
        private SlidingPopup _popup;
        private RectTransform _window;
        private Vector2 _initialPosition;
        private Button _button;
        private CanvasGroup _buttonHider;
        private CanvasGroup _canvasGroup;
        
        public OnHide OnForceHide { get; } = new OnHide();
        
        public bool Free { get; private set; }
        public RectTransform Window => _window;

        private void Awake()
        {
            _popup = GetComponent<SlidingPopup>();
            _window = GetComponent<RectTransform>();
            _button = GetComponentInChildren<Button>();
            _buttonHider = _button.GetComponent<CanvasGroup>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _initialPosition = transform.position;
            _buttonHider.alpha = 0;
        }

        public void Show(string message, Sprite icon)
        {
            Free = false;
            notifMessage.text = message;
            timeStamp.text = GameTime.Clock.CurrentTimeFormat;

            if (icon != null)
                notifIcon.sprite = icon;

            notifIcon.gameObject.SetActive(icon != null);
            
            _popup.Show(null);
        }

        public void Hide()
        {
            _popup.Hide(() =>
            {
                Free = true;
                ResetPosition();
                transform.localScale = Vector3.one;
                _button.interactable = false;
                _buttonHider.alpha = 0;
            });
        }
        
        public void ForceHide()
        {
            OnForceHide.Invoke(this);
            Hide();
        }
        
        public void MoveUp()
        {
            const float cardSpaceRatio = 0.05f;
            
            Vector2 movePosition = transform.position;
            var cardHeight = GetCardHeight();
            var spacing = cardHeight * cardSpaceRatio;
            movePosition.y += cardHeight + spacing;

            MoveCard(movePosition, 1);
        }
        
        public void MoveUpOnDock()
        {
            Vector2 movePosition = transform.position;
            var cardHeight = GetCardHeight();
            movePosition.y += cardHeight;

            MoveCard(movePosition, dockScaleRatio);
        }

        public void Dock(Vector2 dockPosition)
        {
            Vector2 movePosition = transform.position;
            movePosition.y = dockPosition.y;
            
            MoveCard(movePosition, dockScaleRatio, () =>
            {
                _button.interactable = true;
                _buttonHider.alpha = 1;
                _canvasGroup.alpha = HidingAlpha;
            });
        }
        
        public float GetCardHeight()
        {
            var corners = new Vector3[4];
            _window.GetWorldCorners(corners);
            return Mathf.Abs(corners[0].y - corners[1].y);
        }

        private void MoveCard(Vector2 movePosition, float scaleRatio, UnityAction actionOnMove = null)
        {
            _popup.Move(movePosition, Vector3.one * scaleRatio, actionOnMove);
            UpdateHiddenPosition(movePosition.y);
        }

        private void UpdateHiddenPosition(float verticalOffset)
        {
            var updatedHiddenPosition = hidePosition.position;
            updatedHiddenPosition.y = verticalOffset;
            hidePosition.position = updatedHiddenPosition;
        }
        
        private void ResetPosition()
        {
            transform.position = _initialPosition;
            UpdateHiddenPosition(_initialPosition.y);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_button.interactable) return;

            _canvasGroup.alpha = 1;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_button.interactable) return;
            
            _canvasGroup.alpha = HidingAlpha;
        }
    }

    public class OnHide : UnityEvent<NotificationCard>
    { }
}