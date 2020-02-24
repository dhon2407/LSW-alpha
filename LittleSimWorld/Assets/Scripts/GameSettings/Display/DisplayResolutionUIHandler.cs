using GameSettings.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DropDown = TMPro.TMP_Dropdown;
using DropDownData = TMPro.TMP_Dropdown.OptionData;

namespace GameSettings
{
    public class DisplayResolutionUIHandler : MonoBehaviour
    {
        [SerializeField]
        private DropDown dropDownList;
        private List<Resolution> resolutions;

        private void Awake()
        {
            dropDownList.ClearOptions();
            resolutions = new List<Resolution>();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }

        private IEnumerator Initialize()
        {
            while (!Settings.DataReady)
                yield return null;

            dropDownList.onValueChanged.AddListener(delegate { UpdateResolution(); });
            Settings.Display.onResolutionListChanged.AddListener(UpdateList);

            UpdateList();
        }

        public void UpdateResolution()
        {
            if (resolutions.Count > 0 && !Settings.Display.AutoDetectResolution)
                Settings.Display.ChangeResolution(resolutions[dropDownList.value]);
        }

        private void UpdateList()
        {
            ClearValues();

            if (Settings.Display.CurrentAspectRatio != (0,0))
            {
                foreach (var res in Settings.Display.Resolutions)
                {
                    dropDownList.options.Add(new DropDownData(string.Format("{0} x {1}",
                                                              res.width, res.height)));
                    resolutions.Add(res);
                }

                SelectCurrentResolution();
            }
        }

        private void SelectCurrentResolution()
        {
            var resolution = Settings.Display.CurrentGameResolution;
            if (resolutions.Exists(res => res.Same(resolution)))
            {
                var index = resolutions.FindIndex(res => res.Same(resolution));
                dropDownList.value = index == 0 ? -1 : index;
            }
            else
            {
                dropDownList.value = resolutions.Count == 1 ? -1 : resolutions.Count - 1;
                UpdateResolution();
            }
        }

        private void OnDestroy()
        {
            Settings.Display.onResolutionListChanged.RemoveListener(UpdateList);
        }

        private void Reset()
        {
            dropDownList = GetComponent<DropDown>();
        }

        private void ClearValues()
        {
            resolutions.Clear();
            dropDownList.ClearOptions();
        }
    }
}