using System;
using System.Collections;
using GUI_Animations;
using UnityEngine;
using UnityEngine.Events;

namespace LSW.Tooltip
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class TooltipPopup : MonoBehaviour, IUiPopup
    {
        [SerializeField] protected float popScale = 1f;
        [SerializeField] protected float popDuration = 0.05f;
        [SerializeField] protected LeanTweenType popStyle = LeanTweenType.linear;
        
        [Space, Header("Mouse Offset")]
        [SerializeField] private float mouseOffsetY = 32;
        [SerializeField] private int referenceScreenHeight = 720;

        public void Show(Vector2 position, UnityAction actionOnOpen)
        {
            if (_animating) StopAllCoroutines();
            
            StartCoroutine(PopIn(position, actionOnOpen));
        }
        
        public void Hide(UnityAction actionOnClose)
        {
            if (_animating) StopAllCoroutines();

            StartCoroutine(PopOut(actionOnClose));
        }
        
        public void Move(Vector2 screenPosition)
        {
            UpdatePosition(screenPosition);
            UpdateDisplayPivot(true);
        }
        
        public void ToggleState()
        { } //NOT NEEDED FOR TOOLTIPS

        public bool Visible => _isVisible;
        public UnityEvent OnShow { get; } = null;
        public UnityEvent OnHide { get; } = null;

        public void Show(UnityAction actionOnOpen)
        { } //NOT NEEDED FOR TOOLTIPS
        
        public void Hide(Vector2 position, UnityAction actionOnClose)
        { } //NOT NEEDED FOR TOOLTIPS

        private RectTransform _window;
        private CanvasGroup _canvasGroup;
        private Camera _cam;
        private bool _animating;
        private Corners _cropCorners = Corners.None;
        private Vector2 _mouseOffset = Vector2.zero;
        private bool _isVisible;

        private float WidthOverlapRatio => GetXAxisOverlapRatio();
        private Vector3 VectorPopInScale => Vector3.one * popScale;
        private Vector2 MouseOffset => GetMouseOffset();
        
        private void Start()
        {
            Reset();
            _window.localScale = Vector3.zero;
            _animating = false;
            _cam = Camera.main;
        }
       
        private IEnumerator PopOut(UnityAction actionOnClose)
        {
            _animating = true;
            
            Animate(Vector3.zero, 0);
            yield return new WaitForSecondsRealtime(popDuration);

            _animating = false;
            _isVisible = false;
            actionOnClose?.Invoke();
        }

        private IEnumerator PopIn(Vector2 position, UnityAction actionOnOpen)
        {
            _animating = true;
            
            yield return CheckTooltipBounds(position);
            UpdateDisplayPivot();
            
            LeanTween.scale(_window, Vector3.zero, 0);
            Animate(VectorPopInScale, 1);
            yield return new WaitForSecondsRealtime(popDuration);

            _animating = false;
            _isVisible = true;
            actionOnOpen?.Invoke();
        }

        private void UpdateDisplayPivot(bool refresh = false)
        {
            if (!refresh && _cropCorners == Corners.None)
            {
                _window.pivot = Vector2.up;
                ApplyOffset();
                return;
            }
            
            var newPivot = _window.pivot;

            if (_cropCorners.HasFlag(Corners.BottomLeft) && _cropCorners.HasFlag(Corners.BottomRight))
                newPivot.y = 0;
            
            if (_cropCorners.HasFlag(Corners.TopRight) && _cropCorners.HasFlag(Corners.BottomRight))
                newPivot.x = 1 * WidthOverlapRatio;
            
            _window.pivot = newPivot;
        }

        private IEnumerator CheckTooltipBounds(Vector2 displayPosition)
        {
            _window.pivot = Vector2.up;
            _window.localScale = VectorPopInScale;
            _canvasGroup.alpha = 0f;

            yield return null;

            UpdatePosition(displayPosition);
        }

        private void UpdatePosition(Vector2 displayPosition)
        {
            transform.position = displayPosition;
            UpdateOverlappingCorners();
            ApplyOffset();
        }

        private void UpdateOverlappingCorners()
        {
            _cropCorners = Corners.None;
            var corners = new Vector3[4];
            _window.GetWorldCorners(corners);

            for (int cornerIndex = 0; cornerIndex < 4; cornerIndex++)
            {
                var viewPortPosition =
                    _cam.ScreenToViewportPoint((Vector2) corners[cornerIndex] + MouseOffset);
                
                if (OnViewPort(viewPortPosition))
                    continue;

                _cropCorners |= GetCorner(cornerIndex);
            }
        }

        private void ApplyOffset()
        {
            Vector2 currentPosition = transform.position;
            currentPosition += MouseOffset;
            transform.position = currentPosition;
        }
        
        private Vector2 GetMouseOffset()
        {
            _mouseOffset = Vector2.zero;
            
            if (_window.pivot.y >= 1)
                _mouseOffset.y -= (mouseOffsetY * Screen.height) / referenceScreenHeight;

            return _mouseOffset;
        }

        private static Corners GetCorner(int cornerIndex)
        {
            switch (cornerIndex)
            {
                case 0: return Corners.BottomLeft;
                case 1: return Corners.TopLeft;
                case 2: return Corners.TopRight;
                case 3: return Corners.BottomRight;
                
                default: return Corners.None;
            }
        }

        private static bool OnViewPort(Vector3 corner)
        {
            return (corner.x >= 0 && corner.x <= 1) && (corner.y >=0 && corner.y <=1);
        }

        private void Animate(Vector3 scale, float alpha)
        {
            LeanTween.scale(_window, scale, popDuration).setEase(popStyle);
            LeanTween.alphaCanvas(_canvasGroup, alpha, popDuration);
        }
        
        private float GetXAxisOverlapRatio()
        {
            var mouseToEdge = Mathf.Abs(Screen.width - _window.position.x);
            var corners = new Vector3[4];
            _window.GetWorldCorners(corners);

            var rectWidth = Mathf.Abs(corners[0].x - corners[3].x);
            var overlapWidth =  Mathf.Abs(rectWidth - mouseToEdge);

            return float.IsInfinity(overlapWidth / rectWidth) ? 1 : (overlapWidth / rectWidth);
        }

        private void Reset()
        {
            _window = GetComponent<RectTransform>();
            _canvasGroup = _window.GetComponent<CanvasGroup>();
            _canvasGroup.blocksRaycasts = false;
        }

        [System.Flags]
        private enum Corners
        {
            None = 0,
            BottomLeft = 1,
            TopLeft = 2,
            TopRight = 4,
            BottomRight = 8, 
        }
    }
}