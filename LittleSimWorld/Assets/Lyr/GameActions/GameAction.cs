using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameActions {

	public abstract class GameAction {
		public abstract void PerformAction();
	}

	[System.Flags] public enum Stats { Health, Stamina, Hygiene, Bladder }
	[System.Flags] public enum Skills { Invalid, Fitness, Cooking, etc }
	public enum MathOperationType { FlatPerSecond, FlatPerFrame }

	public class AddStatAction : GameAction {

		[HorizontalGroup("A")] public Stats StatToAdd;
		[HorizontalGroup("A"), MinValue(0)] public int Amount;

		public override void PerformAction() {
			// StatsManager.AddStat(Amount);
		}
	}

	public class DepletStatAction : GameAction {

		[HorizontalGroup("A")] public Stats StatsToRemove;
		[HorizontalGroup("A"), MinValue(0)] public int Amount;

		public override void PerformAction() {
			// StatsManager.DepletStat(Amount);
		}
	}

	public class IncreaseSkillAction : GameAction {

		[HorizontalGroup("A")] public Skills StatsToIncrease;
		[HorizontalGroup("A"), MinValue(0)] public int Amount;

		public override void PerformAction() {
			// StatsManager.AddStat(Amount);
		}
	}

	public class DecreaseSkillAction : GameAction {

		[HorizontalGroup("A")] public Skills SkillsToSubstract;
		[HorizontalGroup("A"), MinValue(0)] public int Amount;

		public override void PerformAction() {
			// StatsManager.DepletStat(Amount);
		}
	}


}