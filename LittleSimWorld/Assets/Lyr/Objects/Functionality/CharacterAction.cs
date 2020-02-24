using System.Collections;
using System.Collections.Generic;
using CharacterStats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Objects.Functionality {
	[HideReferenceObjectPicker, LabelFromMethod("ToString")]
	public class CharacterAction {
		public override string ToString() => ActionName;

		public string ActionName;
		//public string AnimationName;
		[Tooltip("Delay after the use exits without any user input.")]
		[Range(0, 10)] public float DelayAfterFinish;



		// The lists here are purely cosmetic .. stats can go on both lists.
		[Space, Header("Status Validating")]
		public StatusRequirementList MinimumStatsToUse = new StatusRequirementList("Minimum Stats To Use");
		public StatusRequirementList MaximumStatsToUse = new StatusRequirementList("Maximum Stats To Use");

		public bool ValidateStats() => MinimumStatsToUse.Validate() && MaximumStatsToUse.Validate();

		public string GetErrorMessage() {
			if (!MinimumStatsToUse.Validate()) { return MinimumStatsToUse.GetErrorMessage(); }
			else if (!MaximumStatsToUse.Validate()) { return MaximumStatsToUse.GetErrorMessage(); }
			else {
				Debug.LogError("You're asking for error message when the current stats are valid.");
				return null;
			}
		}

		[Space, Header("Status Modifying")]
		public AttributeModifierList AttributesChangeOnUse = new AttributeModifierList("On Use | Added each frame");
		public AttributeModifierList AttributesChangeOnFinish = new AttributeModifierList("On Finish | Added flat");

		[Space, Header("Action Effects")]

		public List<ActionEffect> actionEffects = new List<ActionEffect>();

		/// <summary>
		/// Begin the action and activate the effects.
		/// </summary>
		public void BeginAction() => actionEffects.ForEach(x => x.Activate());

		/// <summary>
		/// Ends the action and terminates the effects.
		/// </summary>
		/// <param name="canceled">Was the action canceled?</param>
		public void EndAction(bool canceled = false) => actionEffects.ForEach(x => x.Deactivate(canceled));

		/// <summary>
		/// Apply the Attribute Changes that are specified in the editor as AttributesChangeOnUse.
		/// <para>Time is automatically calculated to match the game time.</para>
		/// </summary>
		public void ApplyStatsOnUse() => AttributesChangeOnUse.Apply();
		/// <summary>
		/// Apply the Attribute Changes that are specified in the editor as AttributesChangeOnUse.
		/// </summary>
		/// <param name="t">Manual time, in in-game hours.</param>
		public void ApplyStatsOnUse(float t) => AttributesChangeOnUse.ApplyFlat(t);

		/// <summary>
		/// Apply the Attribute Changes that are specified in the editor as AttributesChangeOnFinish.
		/// </summary>
		public void ApplyStatsOnFinish() => AttributesChangeOnFinish.Apply();
		/// <summary>
		/// Apply the Attribute Changes that are specified in the editor as AttributesChangeOnFinish.
		/// </summary>
		/// <param name="t">Manual time, in in-game hours.</param>
		public void ApplyStatsOnFinish(float t) => AttributesChangeOnFinish.ApplyFlat(t);

	}
}