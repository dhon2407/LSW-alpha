using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayAspectRatioUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList = null;
        private List<(int, int)> ratios;

        private void Awake()
        {
            ratios = new List<(int, int)>();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            Settings.Display.onValuesChanged.AddListener(UpdateList);
            dropDownList.onValueChanged.AddListener(delegate { ChangeValue(); });
        }

        private void ChangeValue()
        {
            if (!Settings.Display.AutoDetectResolution)
                Settings.Display.SetAspectRatio(ratios[dropDownList.value]);
        }

        private void UpdateList()
        {
            ClearItems();

            foreach (var ratio in Settings.Display.AspectRatios)
            {
                ratios.Add(ratio);
                dropDownList.options.Add(new DropDownData(string.Format("{0}:{1}",
                    ratio.Item1, ratio.Item2)));
            }

            SetCurrentRatio();
        }

        private void SetCurrentRatio()
        {
            var ratio = Settings.Display.ActualAspectRatio;
            if (ratios.Contains(ratio))
            {
                var index = ratios.IndexOf(ratio);
                dropDownList.value = index == 0 ? -1 : index;
                ChangeValue();
            }
        }

        private void ClearItems()
        {
            ratios.Clear();
            dropDownList.ClearOptions();
        }

        public void SetToEnable(bool enable)
        {
            if (enable)
                RefreshList();
        }

        private void RefreshList()
        {
            Settings.Display.CustomResolution();
            UpdateList();
        }
    }
}