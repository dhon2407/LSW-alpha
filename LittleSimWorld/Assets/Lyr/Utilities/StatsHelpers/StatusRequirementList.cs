namespace CharacterStats {
	using System.Collections;
	using System.Collections.Generic;
	using PlayerStats;
	using Sirenix.OdinInspector;
	using UnityEngine;

	/// <summary> List of status requirements. </summary>
	[DrawChildElements, HideReferenceObjectPicker]
	public class StatusRequirementList {

		[SerializeField, HideInInspector] string Name;
		public StatusRequirementList(string name) => Name = name;

		[LabelText("@Name"), ListDrawerSettings(AlwaysAddDefaultValue = true)]
		public List<StatusRequirement> requirements = new List<StatusRequirement>();

		/// <summary> Checks and validates if all requirements are satisfied.. </summary>
		public bool Validate() {
			foreach (var req in requirements) {
				if (!req.Validate()) { return false; }
			}

			return true;
		}

		public string GetErrorMessage() {
			foreach (var req in requirements) {
				if (!req.Validate()) { return req.ErrorMessage; }
			}
			Debug.LogError("Stats are valid but you're requesting an error message.");

			return "";
		}

	}

}