using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField]
        private Toggle autoDetect = null;
        [SerializeField]
        private Toggle customDisplay = null;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            UpdateData();
            Invoke(nameof(OverrideFPSVSync), 1f);
        }

        private void UpdateData()
        {
            autoDetect.isOn = Settings.Display.AutoDetectResolution;
            customDisplay.isOn = !autoDetect.isOn;
        }

        private void OverrideFPSVSync()
        {
            Settings.Display.RefreshFPSVSyncValues();
        }

        public void SaveSettings()
        {
            Settings.Save();
        }

        public void Cancel()
        {
            Settings.Load();
            UpdateData();
        }

        public void LoadDefault()
        {
            Settings.DefaultSettings();
            UpdateData();
        }
    }
}