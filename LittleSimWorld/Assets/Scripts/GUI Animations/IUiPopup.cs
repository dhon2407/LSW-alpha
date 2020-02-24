using UnityEngine;
using UnityEngine.Events;

namespace GUI_Animations
{
    public interface IUiPopup
    {
        void Show(UnityAction actionOnOpen);
        void Show(Vector2 screenPosition, UnityAction actionOnOpen);
        void Hide(UnityAction actionOnClose);
        void Hide(Vector2 screenPosition, UnityAction actionOnClose);
        void Move(Vector2 screenPosition);
        void ToggleState();
        bool Visible { get; }
        UnityEvent OnShow { get; }
        UnityEvent OnHide { get; }
    }
}