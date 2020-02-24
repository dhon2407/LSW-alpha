using CharacterData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {
	public class CharacterPanel : MonoBehaviour {

		public Button LeftButton, RightButton, RandomizeButton, FemaleGenderButton, MaleGenderButton;

		[Space, Header("Selection Panel Settings")]
		public RectTransform SelectionPanel;
		Vector3 originalPanelPos;
		public float Offset = 81;


		void Awake() {
			LeftButton.onClick.AddListener(ChangeGender);
			RightButton.onClick.AddListener(ChangeGender);
			RandomizeButton.onClick.AddListener(Randomize);
			MaleGenderButton.onClick.AddListener(ChangeGender);
			FemaleGenderButton.onClick.AddListener(ChangeGender);

			originalPanelPos = SelectionPanel.transform.localPosition;
		}

		public delegate void RandomizeEvent();
		public static event RandomizeEvent DoRandomize;

		void Randomize() {
			DoRandomize();
			// We don't want it to randomize gender either.
			//var rnd = Random.Range(-1, 2);
			//if (rnd == 0) { ChangeGender(); }
		}

		void ChangeGender() {
			CharacterCreationManager.instance.ChangeGender(ArrowType.Left);
			SetSelectionPanelPosition();
			FemaleGenderButton.interactable = CharacterCreationManager.instance.gender == Gender.Male;
			MaleGenderButton.interactable = CharacterCreationManager.instance.gender == Gender.Female;
		}

		[Button]
		void SetSelectionPanelPosition() {
			var targetPos = originalPanelPos;
			if (CharacterCreationManager.instance.gender == Gender.Female) { targetPos += Offset * Vector3.right; }
			SelectionPanel.transform.localPosition = targetPos;

		}
	}
}