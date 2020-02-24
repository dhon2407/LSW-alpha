using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GUI_Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SlidingPopup : MonoBehaviour, IUiPopup
    {
        [SerializeField] private bool hideAtStart = true;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private Transform sourcePosition = null;

        public void Move(Vector2 screenPosition) { }
        public bool Visible => _isVisible;
        public UnityEvent OnShow { get; } = new UnityEvent();
        public UnityEvent OnHide { get; } = new UnityEvent();
        
        public void Show(UnityAction actionOnOpen)
        {
            Show(_targetPosition, actionOnOpen);
        }

        public void Show(Vector2 screenPosition, UnityAction actionOnOpen)
        {
            _targetPosition = screenPosition;
            if (!_isVisible && !_isAnimating)
                StartCoroutine(SlideIn(actionOnOpen));
        }

        public void Hide(UnityAction actionOnClose)
        {
            Hide(_hiddenPosition, actionOnClose);
        }

        public void Hide(Vector2 screenPosition, UnityAction actionOnClose)
        {
            if (_isVisible && !_isAnimating)
                StartCoroutine(SlideOut(actionOnClose));
        }

        public void ForceHide(UnityAction actionOnClose)
        {
            StopAllCoroutines();
            StartCoroutine(ForceSlideOut(actionOnClose));
        }

        public void Move(Vector2 screenPosition, Vector3 targetScale, UnityAction action)
        {
            _targetPosition = screenPosition;
            StartCoroutine(MoveOut(action, targetScale));
        }

        public void ToggleState()
        {
            if (_isVisible)
                Hide(null);
            else
                Show(null);
        }
        
        private CanvasGroup _canvasGroup;
        private RectTransform _mainWindow;
        
        private bool _isVisible;
        private bool _isAnimating;
        private Vector2 _targetPosition;
        private Vector2 _hiddenPosition;

        private Vector3 _initialScale;
        
        private IEnumerator SlideIn(UnityAction actionOnOpen)
        {
            RefreshLocation();
            transform.position = _hiddenPosition;
            transform.localScale = _initialScale;
            _canvasGroup.alpha = 0;

            _isAnimating = true;
            LeanTween.move(_mainWindow.gameObject, _targetPosition, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 1, duration);
            
            yield return new WaitForSeconds(duration);

            _isAnimating = false;
            _isVisible = true;
            actionOnOpen?.Invoke();
            OnShow.Invoke();
        }

        private IEnumerator SlideOut(UnityAction actionOnClose)
        {
            RefreshLocation();
            
            _isAnimating = true;
            LeanTween.move(_mainWindow.gameObject, _hiddenPosition, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 0, duration);
            
            yield return new WaitForSeconds(duration);

            _isAnimating = false;
            _isVisible = false;
            transform.localScale = Vector3.zero;
            transform.position = _targetPosition;
            actionOnClose?.Invoke();
            OnHide.Invoke();
        }
        
        private IEnumerator ForceSlideOut(UnityAction actionOnClose)
        {
            _isAnimating = true;
            LeanTween.move(_mainWindow.gameObject, _hiddenPosition, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 0, duration);
            
            yield return new WaitForSeconds(duration);

            _isAnimating = false;
            _isVisible = false;
            transform.localScale = Vector3.zero;
            transform.position = _targetPosition;
            actionOnClose?.Invoke();
            OnHide.Invoke();
        }
        
        private IEnumerator MoveOut(UnityAction action, Vector3 targetScale)
        {
            LeanTween.move(_mainWindow.gameObject, _targetPosition, duration).setEase(LeanTweenType.linear);
            LeanTween.scale(_mainWindow, targetScale, duration).setEase(LeanTweenType.linear);
            
            yield return new WaitForSeconds(duration);
            action?.Invoke();
        }

        private void RefreshLocation()
        {
            _hiddenPosition = sourcePosition.position;
            _targetPosition = transform.position;
        }

        private void Awake()
        {
            _mainWindow = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _hiddenPosition = _targetPosition = transform.position;
            _initialScale = transform.localScale;

            if (hideAtStart)
            {
                _canvasGroup.alpha = 0;
                transform.localScale = Vector3.zero;
                _hiddenPosition = sourcePosition.position;
            }

            _isVisible = !hideAtStart;
        }
    }
}