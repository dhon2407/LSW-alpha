using System;
using GUI_Animations;
using UI.Buttons;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject menu = null;
        private IUiPopup _popupMenu;

        private Button _button;
        private LSWButton _lswButton;

        public void ToggleMenuState()
        {
            if (_popupMenu != null)
            {
                if (_popupMenu.Visible)
                {
                    _popupMenu.Hide(null);
                    _lswButton.ReleasePress();
                }
                else
                {
                    _popupMenu.Show(null);
                    _lswButton.HoldPress();
                }
            }
            else
            {
                Debug.LogWarning($"No IUiPopup component attached to game object [{menu.name}].");
            }
        }
        
        private void Start()
        {
            _button = GetComponent<Button>();
            _lswButton = GetComponent<LSWButton>();
            _button.onClick.AddListener(ToggleMenuState);
            _popupMenu = menu.GetComponent<IUiPopup>();
            
            GamePauseHandler.SubscribeCloseEvent(EscCloseEvent);
        }

        private bool EscCloseEvent()
        {
            if (!_popupMenu.Visible) return false;

            _popupMenu.Hide(null);
            _lswButton.ReleasePress();
            return true;
        }
    }
}