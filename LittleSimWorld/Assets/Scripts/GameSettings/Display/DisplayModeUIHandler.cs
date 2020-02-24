using GameSettings.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayModeUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList;
        private List<FullScreenMode> modes = null;

        public void SetToEnable(bool enable)
        {
            if (enable)
                RefreshList();
            else
                dropDownList.ClearOptions();
        }

        private void Awake()
        {
            modes = new List<FullScreenMode>();
            dropDownList.ClearOptions();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            RefreshList();

            Settings.Display.onValuesChanged.AddListener(RefreshList);
            
            dropDownList.onValueChanged.AddListener(delegate { UpdateMode(); });
        }

        private void RefreshList()
        {
            modes.Clear();
            dropDownList.options.Clear();

            foreach (var mode in Settings.Display.Modes)
            {

#if UNITY_STANDALONE_WIN
                if (mode == FullScreenMode.MaximizedWindow)
                    continue;
#endif

                modes.Add(mode);
                dropDownList.options.Add(new DropDownData(mode.NiceString()));
            }

            UpdateValue();

        }

        private void UpdateValue()
        {
            if (modes.Contains(Settings.Display.CurrentFullScreenMode))
            {
                var index = modes.IndexOf(Settings.Display.CurrentFullScreenMode);
                dropDownList.value = index == 0 ? -1 : index;
            }
        }

        public void UpdateMode()
        {
            Settings.Display.ChangeMode(modes[dropDownList.value]);
        }

        private void Reset()
        {
            dropDownList = GetComponent<DropDown>();
        }
    }
}