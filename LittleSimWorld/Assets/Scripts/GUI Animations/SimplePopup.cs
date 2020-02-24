using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GUI_Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SimplePopup : MonoBehaviour, IUiPopup
    {
        [SerializeField] private bool hideAtStart = true;
        [SerializeField] private float duration = 0.1f;
        
        private CanvasGroup _canvasGroup;
        private RectTransform _mainWindow;
        private Vector3 _initialScale;
        private bool _isVisible;

        public void Show(UnityAction actionOnOpen)
        {
            if (!_isVisible)
                StartCoroutine(PopIn(actionOnOpen));
        }

        public void Show(Vector2 screenPosition, UnityAction actionOnOpen)
        {
            Show(actionOnOpen);
        }

        public void Hide(UnityAction actionOnClose)
        {
            if (_isVisible)
                StartCoroutine(PopOut(actionOnClose));
        }

        public void Hide(Vector2 screenPosition, UnityAction actionOnClose)
        {
            Hide(actionOnClose);
        }

        public void Move(Vector2 screenPosition) { }
        //NOT NEEDED
        
        public void ToggleState()
        {
            if (_isVisible)
                Hide(null);
            else
                Show(null);
        }

        public bool Visible => _isVisible;
        public UnityEvent OnShow { get; } = new UnityEvent();
        public UnityEvent OnHide { get; } = new UnityEvent();

        private void Start()
        {
            _mainWindow = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _initialScale = _mainWindow.localScale;
            
            if (hideAtStart)
                _mainWindow.localScale = Vector3.zero;

            _isVisible = !hideAtStart;
        }
        
        private IEnumerator PopIn(UnityAction actionOnOpen = null)
        {
            yield return null;
            _mainWindow.localScale = Vector3.zero;

            LeanTween.scale(_mainWindow, _initialScale, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 1, duration);

            yield return new WaitForSecondsRealtime(duration);

            _isVisible = true;
            actionOnOpen?.Invoke();
            OnShow.Invoke();
        }
        
        private IEnumerator PopOut(UnityAction actionOnClose = null)
        {
            LeanTween.scale(_mainWindow, Vector3.zero, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 0, duration);

            yield return new WaitForSecondsRealtime(duration);

            _isVisible = false;
            actionOnClose?.Invoke();
            OnHide.Invoke();
        }
    }
}