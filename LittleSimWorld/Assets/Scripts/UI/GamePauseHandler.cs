using System;
using System.Collections.Generic;
using GameTime;
using GUI_Animations;
using UI.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GamePauseHandler : MonoBehaviour
    {
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        [SerializeField] private Button triggerButton = null;
        [SerializeField] private GameObject pausePopup = null;
        [SerializeField] private GameObject pauseScreen = null;
        
        private IUiPopup _popupMenu;
        private static List<Func<bool>> _closeFunctions = new List<Func<bool>>();

        private LSWButton _lswButton;
        
        public static void SubscribeCloseEvent(Func<bool> forceCloseEvent)
        {
            _closeFunctions.Add(forceCloseEvent);
        }
        
        public static void UnSubscribeCloseEvent(Func<bool> forceCloseEvent)
        {
            _closeFunctions.Remove(forceCloseEvent);
        }
        

        private void Awake()
        {
            if (pausePopup != null)
            {
                _popupMenu = pausePopup.GetComponent<IUiPopup>();
                _popupMenu.OnHide.AddListener(Clock.Unpause);
            }
            else
            {
                Debug.LogError("Pause popup not found. Pausing disabled.");
                return;
            }

            if (triggerButton != null)
                triggerButton.onClick.AddListener(TryPause);

            _lswButton = GetComponent<LSWButton>();

        }

        private void Start()
        {
            Clock.Pausing += OnPauseEvent;  
        }

        private void Update()
        {
            if (Input.GetKeyDown(pauseKey))
                TogglePause();
        }

        private void TryPause()
        {
            if (_closeFunctions.Count > 0)
            {
                foreach (var closeFunction in _closeFunctions)
                {
                    if (closeFunction())
                        return;
                }
            }

            if (Clock.Paused) return;
            
            _popupMenu.Show(Clock.Pause);
            _lswButton.HoldPress();
        }

        public void TogglePause()
        {
            if (!Clock.Paused)
            {
                TryPause();
            }
            else
            {
                _popupMenu.Hide(Clock.Unpause);
                _lswButton.ReleasePress();
            }
        }

        private void OnPauseEvent(bool isPaused)
        {
            pauseScreen.SetActive(isPaused);
        }
    }
}