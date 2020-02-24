using System.Collections.Generic;
using GameTime;
using UnityEngine;

namespace UI {
	public class MenuBar : MonoBehaviour {
		[SerializeField] private List<KeyShortcuts> keyShortcuts = null;

		private void Start() {
			foreach (KeyShortcuts key in keyShortcuts) {
				key.button.GetComponent<MenuButtonTooltip>()?.SetShortcutKey(key.code);
			}
		}

		private void Update() {
			foreach (KeyShortcuts key in keyShortcuts) {
				if (Input.GetKeyDown(key.code) && !Clock.Paused) {
					key.button.ToggleMenuState();
				}
			}
		}

#pragma warning disable
		[System.Serializable]
		private struct KeyShortcuts {
			public KeyCode code;
			public MenuButton button;
		}
#pragma warning restore
	}
}