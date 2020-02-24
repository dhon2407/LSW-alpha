using GameTime;

namespace CharacterStats {
    using PlayerStats;
    using Sirenix.OdinInspector;
	using UnityEngine;
    using UnityEditor;

	[HideReferenceObjectPicker]
	public abstract class BaseAttributeChangeAction {
		public abstract void PerformAction(float time);
	}

    public class ExtraDrainChangeAction : BaseAttributeChangeAction
    {
        [ShowInInspector, HideLabel] string ToolTip => $"Regular drain multiplied by {multiplierToHourlyDrain} added to {StatToChange} per in-game hour.";

        [Header("Extra Hourly Drain")]
        [LabelText("Stat")] public Status.Type StatToChange;
        [MinValue(0)] public float multiplierToHourlyDrain;

        public override void PerformAction(float time)
        {
            Stats.Status(StatToChange).Drain(time * multiplierToHourlyDrain);
        }
    }

	public class StatusChangeAction : BaseAttributeChangeAction {

		[ShowInInspector, HideLabel] string ToolTip => $"{ActionToPerform} {Amount} {StatToChange} per in-game hour.";

		[ HideLabel, EnumToggleButtons] public MathOperationType ActionToPerform;
		[LabelText("Stat")] public Status.Type StatToChange;
		[ MinValue(0)] public float Amount;

		public override void PerformAction(float time) {
			switch (ActionToPerform) {
				case MathOperationType.Add:
					Stats.Status(StatToChange).Add(Amount * time);
					break;
				case MathOperationType.Substract:
					Stats.Status(StatToChange).Remove(Amount * time);
					break;
				default:
					break;
			}

		}
	}

	public class SkillChangeAction : BaseAttributeChangeAction {

		[ShowInInspector, HideLabel] string ToolTip => $"{ActionToPerform} {Amount} {SkillToChange} per seconds(real time).";

		[HideLabel, EnumToggleButtons] public MathOperationType ActionToPerform;

		[LabelText("Skill")]public PlayerStats.Skill.Type SkillToChange;

		[MinValue(0)] public float Amount;

		public override void PerformAction(float time) {
			switch (ActionToPerform) {
				case MathOperationType.Add:
					Stats.AddXP(SkillToChange, Amount * time * Clock.TimeMultiplier * Clock.Speed);
					break;
				case MathOperationType.Substract:
					Stats.AddXP(SkillToChange, Amount * time * Clock.TimeMultiplier * Clock.Speed);
					break;
				default:
					break;
			}

		}
	}

	public class GoldGainAction : BaseAttributeChangeAction {
		[HorizontalGroup("_"), ShowInInspector, HideLabel] string ToolTip => $"{ActionToPerform} {Amount} gold per in-game hour.";

		[HideLabel, EnumToggleButtons] public MathOperationType ActionToPerform;
		[MinValue(0)] public int Amount;

		public override void PerformAction(float time) => Stats.AddMoney(Amount * time);
	}
}