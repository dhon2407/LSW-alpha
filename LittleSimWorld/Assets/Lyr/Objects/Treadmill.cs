using System.Collections;
using UnityEngine;

using static PlayerStats.Status.Type;
using static PlayerStats.Skill.Type;
using Stats = PlayerStats.Stats;

namespace Objects {
	public class Treadmill : UseableObject {

		public override void Interact() {
			var result = TrySetCurrentAction("Cardio");

			if (result) { PlayerCommands.JumpTo(this); }
			else { result.PrintErrorMessage(); }
		}

		public override void BeginUsing() {
			GameLibOfMethods.cantMove = true;

			CurrentAction.BeginAction();
			useState = ObjectUseState.InUse;
		}
		public override void Use() {
			CurrentAction.ApplyStatsOnUse();
			Player.anim.SetFloat("Horizontal", -1);
			SpriteControler.Instance.UpdateCharacterOrientation(CharacterOrientation.Left);

			if (shouldCancel) { FinishUsing(true); }
			else if (!CurrentAction.ValidateStats()) {
				useState = ObjectUseState.Preparation;
				ExitAfterDelay(false);
			}

		}

		public override void FinishUsing(bool cancel) {
			ResetObjectState(cancel);
			PlayerCommands.JumpOff(0, action);
			void action() => SpriteControler.Instance.ChangeSortingOrder(0);
		}

	}
}