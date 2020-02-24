using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{
    public class DisplayVSyncUIHandler : MonoBehaviour
    {
        [SerializeField]
        private Toggle toggle;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            Settings.Display.onValuesChanged.AddListener(UpdateValue);
            
            UpdateValue();

            toggle?.onValueChanged.AddListener(delegate
            {
                ToggleVSync(toggle);
            });
        }

        private void UpdateValue()
        {
            toggle.isOn = Settings.Display.VSyncOn;
        }

        public void ToggleVSync(Toggle toggle)
        {
            Settings.Display.SetVsync(toggle.isOn);
        }

        private void OnDestroy()
        {
            Settings.Display.onValuesChanged.RemoveListener(UpdateValue);
        }

        private void Reset()
        {
            toggle = GetComponent<Toggle>();
        }
    }
}