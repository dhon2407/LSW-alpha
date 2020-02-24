using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{
    [RequireComponent(typeof(Toggle))]
    public class VolumeMuter : MonoBehaviour
    {
        [SerializeField]
        private bool masterVolume = false;
        [Space]
        [SerializeField]
        private SoundMixer.SoundGroup soundGroup = SoundMixer.SoundGroup.Background;
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

            Settings.Sound.onValuesChanged.AddListener(UpdateValue);

            UpdateValue();

            toggle.onValueChanged.AddListener(
                delegate
                {
                    MuteVolume(toggle);
                });
        }

        private void UpdateValue()
        {
            if (masterVolume)
                toggle.isOn = Settings.Sound.IsMasterVolumeMuted;
            else
                toggle.isOn = Settings.Sound.isVolumeMuted(soundGroup);
        }

        private void MuteVolume(Toggle toggle)
        {
            if (masterVolume)
                Settings.Sound.MuteMasterVolume(toggle.isOn);
            else
                Settings.Sound.MuteVolume(soundGroup, toggle.isOn);
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