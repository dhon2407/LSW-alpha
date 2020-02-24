using PlayerStats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CharacterStats {

	/// <summary> Requirement of a status. </summary>
	[HideReferenceObjectPicker]
	public class StatusRequirement {

		[Tooltip("Status to Compare")]
		public Status.Type Stat;

		[EnumToggleButtons, HideLabel] public MathComparisonType comparison;

		[Range(0, 100)] public int Amount;

		public string ErrorMessage;

		public bool Validate() {
			switch (comparison) {
				case MathComparisonType.EqualTo: 
					return Mathf.FloorToInt(Stats.GetAmount(Stat)) == Amount || Mathf.CeilToInt(Stats.GetAmount(Stat)) == Amount;
				case MathComparisonType.SmallerThan:
					return Stats.GetAmount(Stat) < Amount;
				case MathComparisonType.BiggerThan:
					return Stats.GetAmount(Stat) > Amount;
				default:
					Debug.LogError("Wrong operation");
					return true;
			}
		}

		public static implicit operator bool(StatusRequirement req) => req.Validate();

	}
}