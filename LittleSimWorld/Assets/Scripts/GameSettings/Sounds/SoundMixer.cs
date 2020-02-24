using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameSettings
{
    public class SoundMixer
    {
        private const SoundGroup BG = SoundGroup.Background;
        private const SoundGroup SFX = SoundGroup.SFX;
        private const SoundGroup FOOTSTEP = SoundGroup.Footsteps;

        private Dictionary<SoundGroup, List<AudioSource>> soundSources;
        private Dictionary<SoundGroup, float> soundSourcesVolume;
        private Dictionary<SoundGroup, bool> soundSourcesVolumeMuted;
        private float masterVolume;
        private bool masterVolumeMuted;

        public bool IsMasterVolumeMuted => masterVolumeMuted;
        public bool DataLoaded { get; private set; }

        public UnityEvent onValuesChanged;

        public SoundMixer()
        {
            onValuesChanged = new UnityEvent();
            soundSources = new Dictionary<SoundGroup, List<AudioSource>>();
            soundSourcesVolume = new Dictionary<SoundGroup, float>();
            soundSourcesVolumeMuted = new Dictionary<SoundGroup, bool>();
            masterVolumeMuted = false;

            foreach (SoundGroup group in Enum.GetValues(typeof(SoundGroup)))
            {
                soundSources[group] = new List<AudioSource>();
                soundSourcesVolume[group] = 1;
                soundSourcesVolumeMuted[group] = false;
            }
            
            LoadSettings();
        }

        public void RestoreDefaults()
        {
            ChangeMasterVolume(1);
            MuteMasterVolume(false);

            foreach (SoundGroup group in Enum.GetValues(typeof(SoundGroup)))
            {
                ChangeVolume(group, 0.5f);
                MuteVolume(group, false);
            }

            onValuesChanged.Invoke();
            DataLoaded = true;
        }

        public void RegisterSource(SoundGroup group, AudioSource audioSource)
        {
            if (!soundSources[group].Contains(audioSource))
            {
                soundSources[group].Add(audioSource);
                audioSource.volume = Mathf.Clamp01(soundSourcesVolume[group] * masterVolume);
                audioSource.mute = masterVolumeMuted ? true : soundSourcesVolumeMuted[group];
            }
        }

        public void RemoveSource(SoundGroup soundGroup, AudioSource audioSource)
        {
            if (soundSources[soundGroup].Contains(audioSource))
                soundSources[soundGroup].Remove(audioSource);
        }

        public void ChangeMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);
            AdjustAllVolumes();
        }

        public void ChangeVolume(SoundGroup soundGroup, float value)
        {
            soundSourcesVolume[soundGroup] = value;

            foreach (var audioSource in soundSources[soundGroup])
                audioSource.volume = Mathf.Clamp01(soundSourcesVolume[soundGroup] * masterVolume);
        }

        public void MuteMasterVolume(bool muted)
        {
            masterVolumeMuted = muted;

            foreach (var soundGroup in soundSources.Keys)
            {
                foreach (var audioSource in soundSources[soundGroup])
                    audioSource.mute = muted ? true : soundSourcesVolumeMuted[soundGroup];
            }
        }

        public void MuteVolume(SoundGroup soundGroup, bool muted)
        {
            foreach (var audioSource in soundSources[soundGroup])
                audioSource.mute = masterVolumeMuted ? true : muted;
            
            soundSourcesVolumeMuted[soundGroup] = muted;
        }

        public float GetMasterVolume()
        {
            return masterVolume;
        }

        public bool isVolumeMuted(SoundGroup soundgroup)
        {
            return soundSourcesVolumeMuted[soundgroup];
        }

        public float GetVolume(SoundGroup soundGroup)
        {
            return soundSourcesVolume[soundGroup];
        }

        public void SaveSetting()
        {
            PlayerPrefs.SetFloat(DataSave.MasterVolume, masterVolume);
            PlayerPrefs.SetInt(DataSave.MasterVolumeMute, masterVolumeMuted ? 1 : 0);

            PlayerPrefs.SetFloat(DataSave.VolumeBG, soundSourcesVolume[BG]);
            PlayerPrefs.SetFloat(DataSave.VolumeSFX, soundSourcesVolume[SFX]);
            PlayerPrefs.SetFloat(DataSave.VolumeFOOTSTEP, soundSourcesVolume[FOOTSTEP]);

            PlayerPrefs.SetInt(DataSave.VolumeBGMute, soundSourcesVolumeMuted[BG] ? 1 : 0);
            PlayerPrefs.SetInt(DataSave.VolumeSFXMute, soundSourcesVolumeMuted[SFX] ? 1 : 0);
            PlayerPrefs.SetInt(DataSave.VolumeFOOTSTEPMute, soundSourcesVolumeMuted[FOOTSTEP] ? 1 : 0);

            PlayerPrefs.SetInt(DataSave.Save, 1);
            PlayerPrefs.Save();
        }

        public void LoadSettings()
        {
            if (PlayerPrefs.HasKey(DataSave.Save))
                LoadSoundSettings();
            else
            {
                RestoreDefaults();
                SaveSetting();
            }
        }

        private void LoadSoundSettings()
        {
            ChangeVolume(BG, PlayerPrefs.GetFloat(DataSave.VolumeBG));
            ChangeVolume(SFX, PlayerPrefs.GetFloat(DataSave.VolumeSFX));
            ChangeVolume(FOOTSTEP, PlayerPrefs.GetFloat(DataSave.VolumeFOOTSTEP));

            MuteVolume(BG, PlayerPrefs.GetInt(DataSave.VolumeBGMute) == 1);
            MuteVolume(SFX, PlayerPrefs.GetInt(DataSave.VolumeSFXMute) == 1);
            MuteVolume(FOOTSTEP, PlayerPrefs.GetInt(DataSave.VolumeFOOTSTEPMute) == 1);

            ChangeMasterVolume(PlayerPrefs.GetFloat(DataSave.MasterVolume, 1));
            MuteMasterVolume(PlayerPrefs.GetInt(DataSave.MasterVolumeMute) == 1);

            DataLoaded = true;
        }

        private void AdjustAllVolumes()
        {
            foreach (var soundGroup in soundSources.Keys)
                foreach (var audioSource in soundSources[soundGroup])
                    audioSource.volume = Mathf.Clamp01(soundSourcesVolume[soundGroup] * masterVolume);
        }

        public enum SoundGroup
        {
            Background,
            SFX,
            Footsteps
        }

        private class DataSave
        {
            public readonly static string MasterVolume = "MasterVolume" + test_value;
            public readonly static string VolumeBG = "VolumeBG" + test_value;
            public readonly static string VolumeSFX = "VolumeSFX" + test_value;
            public readonly static string VolumeFOOTSTEP = "VolumeFOOTSTEP" + test_value;
            public readonly static string MasterVolumeMute = "MasterVolumeMute" + test_value;
            public readonly static string VolumeBGMute = "VolumeBGMute" + test_value;
            public readonly static string VolumeSFXMute = "VolumeSFXMute" + test_value;
            public readonly static string VolumeFOOTSTEPMute = "VolumeFOOTSTEPMute" + test_value;
            public readonly static string Save = "SoundDataSaved" + test_value;

            private static string test_value => "v3";
        }
    }
}