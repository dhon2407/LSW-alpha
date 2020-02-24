using UnityEngine;
using System.Collections;

namespace GameSettings {
	[RequireComponent(typeof(AudioSource))]
	public class GameAudioSource : MonoBehaviour {
		[SerializeField]
		private SoundMixer.SoundGroup soundGroup = SoundMixer.SoundGroup.Background;
		[SerializeField]
		private AudioSource audioSource;

		private void Awake() {
			audioSource = GetComponent<AudioSource>();
		}

		private void Start() {
			Settings.Sound.RegisterSource(soundGroup, audioSource);
		}

		private void OnDestroy() {
			Settings.Sound.RemoveSource(soundGroup, audioSource);
		}

		private void Reset() {
			audioSource = GetComponent<AudioSource>();
			if (audioSource == null)
				audioSource = gameObject.AddComponent<AudioSource>();
		}

		public void PlaySound(AudioClip audioClip, bool loop = false, float volumeMulti = 1) {

			if (!audioClip) {
				Debug.LogWarning("PlaySound() was called with empty audioClip");
				return;
			}

			float volume = Settings.Sound.GetVolume(soundGroup) * volumeMulti;
			audioSource.clip = audioClip;
			audioSource.time = 0;
			audioSource.loop = loop;
			audioSource.volume = volume;
			audioSource.Play();
		}

		public void StopPlayingSound(AudioClip clip) {
			if (audioSource.clip != clip) {
				Debug.LogWarning("StopPlayingSound() was called but active audioClip was different");
				return;
			}

			audioSource.Stop();
		}
	}
}