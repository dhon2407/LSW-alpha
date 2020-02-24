using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CharacterData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {
	public class InputStack : MonoBehaviour {

		public Button AcceptButton, DeclineButton;

		[Space]
		public TMPro.TMP_InputField InputField;

		[Space]
		public GameObject DisableThis;


		void Start() {
			AcceptButton.onClick.AddListener(Confirm);
			DeclineButton.onClick.AddListener(Cancel);
		}

		void Confirm() {
			var playerName = InputField.text.Trim(' ');

			if (string.IsNullOrWhiteSpace(playerName)) { 
				// Play UI Failure Sound
			}
			else {
				// Play UI Success Sound
				var charInfo = CharacterCreationManager.CurrentCharacterInfo;
				charInfo.Gender = CharacterCreationManager.instance.gender;
				charInfo.Name = playerName;

				GameFile.Data.Create(playerName, charInfo);

                MainMenu.LoadGameScene();
                CareerUi.Instance?.UpdateJobUi();
            }
		}

		void Cancel() {
			// Play UI Success Sound
			DisableThis.SetActive(false);
		}

	}
}