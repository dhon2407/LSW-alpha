using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {

	public class AttributePanel : SerializedMonoBehaviour {

		public UIAttributeType attributeType;
		public References references;

		int currentNumber;

		void Start() {
			references.HeaderText.text = attributeType.ToString();
			references.MoveLeft.onClick.AddListener(() => PerformAction(ArrowType.Left));
			references.MoveRight.onClick.AddListener(() => PerformAction(ArrowType.Right));

			// Register Actions to Managers
			CharacterCreationManager.instance.OnGenderChanged += () => PerformAction(0);
			CharacterPanel.DoRandomize += ChangeAtRandom;

			// Update the UI
			PerformAction(0);
		}

		void ChangeAtRandom() {
			for (int i = 0; i < 30; i++) {
				int x = Random.Range(-1, 2);
				if (x == 0) { continue; }
				PerformAction((ArrowType) x, false);
			}

			PerformAction(0);
		}

		void PerformAction(ArrowType arrowType,bool updateText=true) {
			int targetIndex = 0;
			CharacterCreationManager.instance.ChangeSprite(attributeType, arrowType, ref targetIndex);
			if (updateText) { references.NumberText.text = (targetIndex + 1).ToString(); }
		}


		[System.Serializable]
		public class References {
			[TitleGroup("Buttons"), FitLabelWidth] public Button MoveLeft, MoveRight;

			[TitleGroup("Text"), FitLabelWidth] public TMPro.TextMeshProUGUI HeaderText, NumberText;
		}
	}
}