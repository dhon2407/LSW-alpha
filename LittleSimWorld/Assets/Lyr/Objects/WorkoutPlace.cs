using UnityEngine;

namespace Objects {
	public class WorkoutPlace : UseableObject {
		public GameObject Weights;

		public override void Interact() {
			var result = TrySetCurrentAction("Lift Weights");

			if (result) { PlayerCommands.JumpTo(this); }
			else { result.PrintErrorMessage(); }
		}

		public override void BeginUsing() {
			GameLibOfMethods.player.GetComponent<SpriteControler>().FaceRIGHT();
			Weights.SetActive(false);
			GameLibOfMethods.cantMove = true;

			CurrentAction.BeginAction();
			useState = ObjectUseState.InUse;
		}
		public override void Use() {
			CurrentAction.ApplyStatsOnUse();

			if (shouldCancel) { FinishUsing(true); }
			else if (!CurrentAction.ValidateStats()) {
				useState = ObjectUseState.Preparation;
				ExitAfterDelay(false);
			}
		}

		public override void FinishUsing(bool cancel) {
			ResetObjectState(cancel);
			Weights.SetActive(true);
			PlayerCommands.JumpOff(0);
		}

	}
}