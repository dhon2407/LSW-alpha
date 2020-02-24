using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameSettings
{
    [RequireComponent(typeof(Slider))]
    public class VolumeAdjuster : MonoBehaviour
    {
        [SerializeField]
        private bool masterVolume = false;
        [Space]
        [SerializeField]
        private SoundMixer.SoundGroup soundGroup = SoundMixer.SoundGroup.Background;
        [SerializeField]
        private Slider slider = null;

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

            slider.onValueChanged.AddListener(
                delegate
                {
                    UpdateVolume(slider);
                });
        }

        private void UpdateValue()
        {
            slider.value = masterVolume ?
                Settings.Sound.GetMasterVolume() :
                Settings.Sound.GetVolume(soundGroup);
        }

        private void UpdateVolume(Slider slider)
        {
            if (masterVolume)
                Settings.Sound.ChangeMasterVolume(slider.value);
            else
                Settings.Sound.ChangeVolume(soundGroup, slider.value);
        }

        private void OnDestroy()
        {
            Settings.Display.onValuesChanged.RemoveListener(UpdateValue);
        }

        private void Reset()
        {
            slider = GetComponent<Slider>();    
        }
    }
}