using System.Collections;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

namespace Objects {
	public class Toilet : BreakableObject , IInteractionOptions{

		int progressBarID;
        MainMenuOptions optionCancel, optionFix;
        List<MainMenuOptions> originalOptions, brokenOptions;

        private void Start()
        {
            originalOptions = gameObject.GetComponent<Interactivity>().options;
            optionCancel = new MainMenuOptions();
            optionCancel.id = 0; optionCancel.optionName = "Cancel"; optionCancel.interactable = true;
            optionFix = new MainMenuOptions();
            optionFix.id = 1; optionFix.optionName = "Fix"; optionFix.interactable = true;
            brokenOptions = new List<MainMenuOptions>();
            brokenOptions.Add(optionCancel); brokenOptions.Add(optionFix);
        }

        public void Interact(int a)
        {
            Interact();
        }
		public override void Interact() {
			if (breakFunctionality.isBroken) {
				var result = TrySetCurrentAction("Repair");
				if (result) { BeginUsing(); }
				else { result.PrintErrorMessage(); }
			}
			else {
				var result = TrySetCurrentAction("Use Toilet");
				if (result) {
                    Player.anim.SetBool("Use Toilet", true);
                    StartCoroutine(DelayJumpTo());
					SpriteControler.Instance.ChangeSortingOrder(1);
					SpriteControler.Instance.SetClothesState(false);
					SpriteControler.Instance.Censor.SetActive(true);
					useState = ObjectUseState.Preparation;
					//PlayerCommands.JumpTo(this);
				}
				else { result.PrintErrorMessage(); }
			}
		}

        IEnumerator DelayJumpTo()
        {
            yield return new WaitForSeconds(1);
            PlayerCommands.JumpTo(this);
        }

		public override void BeginUsing() {
			GameLibOfMethods.cantMove = true;
			GameLibOfMethods.canInteract = false;
			GameLibOfMethods.doingSomething = true;

			CurrentAction.BeginAction();
			useState = ObjectUseState.InUse;

			if (breakFunctionality.isBroken) {
				float GetProgress() => breakFunctionality.RepairProgress;
				progressBarID = ProgressBar.StartTracking("Fix Toilet", GetProgress, 100);
			}
		}

		public override void Use() {
			CurrentAction.ApplyStatsOnUse();

			if (breakFunctionality.isBroken) {
				SpriteControler.Instance.FaceDOWN();

				if (shouldCancel) {
					ResetObjectState(true);
					ProgressBar.HideUI(progressBarID);
					PlayerAnimationHelper.ResetPlayer();
				}
				else if (breakFunctionality.AttemptFix()) {
                    gameObject.GetComponent<Interactivity>().options = originalOptions;
                    breakFunctionality.isBroken = false;
					PlayerAnimationHelper.ResetPlayer();
					useState = ObjectUseState.Unused;
					ResetObjectState(false);
					ProgressBar.HideUI(progressBarID);
				}
			}
			else {
				SpriteControler.Instance.FaceLEFT();

				if (shouldCancel) { FinishUsing(true); }
				else if (!CurrentAction.ValidateStats()) {
					useState = ObjectUseState.Preparation;
					ExitAfterDelay(false);
				}
			}

		}

		public override void FinishUsing(bool cancel) {
			ResetObjectState(cancel);
			if (breakFunctionality.DidBreakDuringLastUse()) { gameObject.GetComponent<Interactivity>().options = brokenOptions; breakFunctionality.Break(); }

			PlayerCommands.JumpOff(0, () => {
				SpriteControler.Instance.ChangeSortingOrder(0);
				SpriteControler.Instance.SetClothesState(true);
				SpriteControler.Instance.Censor.SetActive(false);
			});
		}
	}
}