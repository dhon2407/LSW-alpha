using GUI_Animations;
using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    [RequireComponent(typeof(TooltipPopup))]
    public abstract class Tooltip<T> : MonoBehaviour, ITooltip<T>
    {
        public abstract void SetData(T data);
        protected virtual Vector2 MousePosition => 
            new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        [Inject]
        public void Init(IUiPopup popup)
        {
            _popup = popup;
        }
        
        public virtual void Show()
        {
            if (IsVisible) return;

            _isAnimating = true;
            IsVisible = true;
            _popup.Show(MousePosition, () => _isAnimating = false);
        }

        public virtual void Hide()
        {
            if (!IsVisible) return;

            _isAnimating = true;
            IsVisible = false;
            _popup.Hide(() => _isAnimating = false);
        }

        protected bool IsVisible;
        private bool _isAnimating;

        private RectTransform _rTransform;
        private IUiPopup _popup;
        
        private void LateUpdate()
        {
            if (IsVisible || _isAnimating)
                _popup.Move(MousePosition);
        }
    }
}