namespace Utilities.Testing {
	using Objects.Utilities;
	using PlayerStats;
	using UnityEngine;
	using UnityEngine.UI;

	[DefaultExecutionOrder(10000)]
	public class InGameObjectUpgradeTester : MonoBehaviour {

		public Transform LayoutParent;
		public InGameObjectUpgradeTesterObjectUI Prefab;
		public Button StatResetButton;

		TMPro.TMP_InputField _inputField;
		GameObject _child;

		void Start() {
			var validators = FindObjectsOfType<UpgradeValidator>();
			foreach (var val in validators) {
				var newPrefab = Instantiate(Prefab, LayoutParent);
				newPrefab.SetTarget(val);
			}

			StatResetButton.onClick.AddListener(SetPlayerStatsToHalf);
			_inputField = StatResetButton.GetComponentInChildren<TMPro.TMP_InputField>();
			_child = transform.GetChild(0).gameObject;

			ChangeState(false);
		}

		void ChangeState() => ChangeState(!_child.activeInHierarchy);
		void ChangeState(bool enable) => _child.SetActive(enable);

		void Update() {
			if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
				ChangeState();
			}
			if (!_child.activeInHierarchy) { return; }
			if (_inputField.isFocused) {
				if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
					SetPlayerStatsToHalf();
				}
				return;
			}

			try {
				int amount = int.Parse(_inputField.text);
				int targetAmount = Mathf.Clamp(amount, 5, 100);

				if (amount != targetAmount) { _inputField.text = amount.ToString(); }

			}
			catch { _inputField.text = 50.ToString(); }
		}

		void SetPlayerStatsToHalf() {
			try {
				int amount = int.Parse(_inputField.text);
				amount = Mathf.Clamp(amount, 5, 100);
				_inputField.text = amount.ToString();

				var statusDictionary = FindObjectOfType<PlayerStatsManager>().PlayerStatus;

				foreach (var statKey in statusDictionary.Keys) {
					statusDictionary[statKey].Set(amount);
				}
			}
			catch { _inputField.text = 50.ToString(); }
		}

	}
}