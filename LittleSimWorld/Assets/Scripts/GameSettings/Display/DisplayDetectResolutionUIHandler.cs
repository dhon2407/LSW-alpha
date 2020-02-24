using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{

    public class DisplayDetectResolutionUIHandler : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;

        private void Start()
        {
            toggle?.onValueChanged.AddListener(delegate
            {
                ToggleDetectResolution(toggle);
            });
        }

        private void ToggleDetectResolution(Toggle toggle)
        {
            if (toggle.isOn)
                Settings.Display.DetectResolution();
        }

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }

    }
}
