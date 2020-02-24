namespace CharacterStats {

	using System.Collections.Generic;
	using PlayerStats;
	using Sirenix.OdinInspector;
	using Sirenix.Serialization;
	using UnityEngine;


	/// <summary> List of status or skill modifications.</summary>
	[DrawChildElements, HideReferenceObjectPicker]
	public class AttributeModifierList {

		[SerializeField, HideInInspector] string Name;
		public AttributeModifierList(string name) => Name = name;

		[ListDrawerSettings(AlwaysAddDefaultValue = true), LabelText("@Name")]
		public List<BaseAttributeChangeAction> Actions = new List<BaseAttributeChangeAction>();

		/// <summary>
		/// <para>Apply the action for the current frame using in-game time.</para>
		/// <para>See also: <see cref="ApplyFlat(float)"/></para>
		/// </summary>
		public void Apply() {
			float t = GameTime.Clock.InGameHoursSinceLastFrame;
			foreach (var attributeAction in Actions) {
				attributeAction.PerformAction(t);
			}
		}

		/// <summary>
		/// Apply flat attribute change for a set amount of in game hours.
		/// </summary>
		/// <param name="seconds">How many seconds of xp are applied.</param>
		public void ApplyFlat(float seconds) {
			if (seconds < 0) {
				Debug.LogError("Negative number cannot be used as time.");
				return;
			}
			else if (seconds == 0) {
				Debug.LogWarning("Zero passed as time.");
				return;
			}

			foreach (var changeAction in Actions) {
				changeAction.PerformAction(seconds);
			}
		}

	}
}