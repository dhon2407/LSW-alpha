using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GUI_Animations
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public class HorizontalGroupPopup : MonoBehaviour, IUiPopup
    {
        [SerializeField] private bool hideAtStart = true;
        [SerializeField] private HorizontalLayoutGroup layoutGroup = null;
        [SerializeField] private RectTransform popOutSourcePosition = null;
        [SerializeField] private float duration = 0.1f;

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

        public void Show(UnityAction actionOnOpen)
        {
            if (!_isVisible && !_animating)
            {
                _mainWindow.gameObject.SetActive(true);
                StartCoroutine(PopIn(actionOnOpen));
            }
        }

        public void Show(Vector2 screenPosition, UnityAction actionOnOpen)
        {
            Show(actionOnOpen);
        }

        public void Hide(UnityAction actionOnClose)
        {
            if (Visible && !_animating)
                StartCoroutine(PopOut(actionOnClose));
        }

        public void Hide(Vector2 screenPosition, UnityAction actionOnClose)
        {
            Hide(actionOnClose);
        }

        public void Move(Vector2 screenPosition) { }

        public void Close()
        {
            Hide(null);
        }

        private RectTransform _mainWindow;
        private CanvasGroup _canvasGroup;
        private Vector2 _sourcePosition;
        private Vector2 _targetPosition;
        private bool _isVisible = false;
        private bool _animating = false;

        private void Start()
        {
            _mainWindow = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            StartCoroutine(GetSourcePosition());
            
            if (hideAtStart)
                _mainWindow.localScale = Vector3.zero;

            _isVisible = !hideAtStart;

            layoutGroup.enabled = true;
        }

        private IEnumerator GetSourcePosition()
        {
            yield return null;
            _sourcePosition = popOutSourcePosition != null
                ? popOutSourcePosition.localPosition
                : _mainWindow.localPosition;
        }

        private IEnumerator PopIn(UnityAction actionOnOpen)
        {
            _animating = true;
            _mainWindow.localScale = Vector3.one;
            _canvasGroup.alpha = 0;

            yield return RefreshHorizontalLayoutGroup();

            _targetPosition = _mainWindow.localPosition;
            _mainWindow.localScale = Vector3.zero;
            _mainWindow.localPosition = _sourcePosition;
            
            LeanTween.move(_mainWindow, _targetPosition, duration);
            LeanTween.scale(_mainWindow, Vector3.one, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 1f, duration);

            yield return new WaitForSecondsRealtime(duration);

            _isVisible = true;
            _animating = false;
            actionOnOpen?.Invoke();
            OnShow.Invoke();
        }

        private IEnumerator RefreshHorizontalLayoutGroup()
        {
            layoutGroup.enabled = false;
            yield return new WaitForSeconds(0.01f);
            layoutGroup.enabled = true;
        }

        private IEnumerator PopOut(UnityAction actionOnClose)
        {
            _animating = true;

            LeanTween.move(_mainWindow, _sourcePosition, duration);
            LeanTween.scale(_mainWindow, Vector3.zero, duration).setEase(LeanTweenType.linear);
            LeanTween.alphaCanvas(_canvasGroup, 0f, duration);

            yield return new WaitForSeconds(duration);
            
            _isVisible = false;
            _animating = false;
            actionOnClose?.Invoke();
            OnHide.Invoke();
            _mainWindow.gameObject.SetActive(false);
            
            yield return RefreshHorizontalLayoutGroup();
        }
    }
}