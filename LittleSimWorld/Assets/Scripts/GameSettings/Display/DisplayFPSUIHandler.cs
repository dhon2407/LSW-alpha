using System;
using System.Collections;
using UnityEngine;

using InputField = TMPro.TMP_InputField;


namespace GameSettings
{
    public class DisplayFPSUIHandler : MonoBehaviour
    {
        [SerializeField] private InputField input;
        [SerializeField] private int minFPS = 30;

        private bool _selected = false;

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            Settings.Display.onValuesChanged.AddListener(UpdateValue);
            Settings.Display.onVSyncChanged.AddListener(VsyncUpdate);
            UpdateValue();
            
            input.onDeselect.AddListener(UpdateFPS);
            input.onSelect.AddListener(arg0 => _selected = true);
            input.onDeselect.AddListener(arg0 => _selected = false);

            VsyncUpdate(QualitySettings.vSyncCount != 0);
        }

        private void Update()
        {
            if (_selected && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
                UpdateFPS(input.text);

        }

        private void VsyncUpdate(bool value)
        {
            input.textComponent.color = value ? Color.red : Color.black;
        }

        private void UpdateValue()
        {
            input.text = Settings.Display.GetFPSLimit().ToString("0");
        }

        private void UpdateFPS(string value)
        {
            if (int.TryParse(value, out int newFPS))
                input.text = Settings.Display.ChangeMaxFPS(Mathf.Clamp(newFPS, minFPS, int.MaxValue)).ToString("0");
            else
                input.text = Settings.Display.ChangeMaxFPS().ToString("0");
        }

        private void OnDestroy()
        {
            Settings.Display.onValuesChanged.RemoveListener(UpdateValue);
        }

        private void Reset()
        {
            input = GetComponent<InputField>();
        }
    }
}