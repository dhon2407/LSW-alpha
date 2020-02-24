using System;
using System.Collections;
using GUI_Animations;
using UnityEngine;

namespace UI
{
    public class TutorialStartup : MonoBehaviour
    {
        private SlidingPopup _popup;

        private void Awake()
        {
            _popup = GetComponent<SlidingPopup>();

            StartCoroutine(CheckStartDisplay());
        }

        private IEnumerator CheckStartDisplay()
        {
            while (!_popup.Visible)
                yield return null;

            if (!PlayerPrefs.HasKey(GameFile.Data.TutorialKey))
            {
                _popup.gameObject.GetComponent<CanvasGroup>().alpha = 0;
                _popup.Hide(null);
            }
            else
                PlayerPrefs.DeleteKey(GameFile.Data.TutorialKey);
        }
    }
}