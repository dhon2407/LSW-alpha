using System.Collections.Generic;
using PlayerStats.Buffs;
using PlayerStats.Buffs.Core;
using UnityEngine;
using GameClock = GameTime.Clock;

namespace Objects {

	public class Bed : UseableObject, IInteractionOptions {

		public SpriteRenderer Covers;

        public void Interact(int a)
        {
            Interact();
        }
		public override void Interact() {
			var result = TrySetCurrentAction("Sleep");
			if (result)
			{
				if (PlayerBuff.HasBuff<Insomnia>())
				{
					PlayerChatLog.Instance.AddChatMessege("I can't sleep.");
					return;
				}
				
				BeginUsing();
			}
			else
			{
				result.PrintErrorMessage();
			}
		}

		public override void BeginUsing() => PlayerCommands.MoveTo(interactionOptions.PlayerStandPosition, delegate {
            JumpToBed().Start();
            Covers.sortingOrder = 2;
        });
		public override void Use() {
			CurrentAction.ApplyStatsOnUse();

			if (shouldCancel && !PlayerBuff.HasBuff<DeepSleep>()) { FinishUsing(true); }
			else if (!CurrentAction.ValidateStats()) {
				useState = ObjectUseState.Preparation;
				GameClock.ResetSpeed();
				ExitAfterDelay(false);
			}

		}
		public override void FinishUsing(bool cancel) {
			//GameFile.Data.Save();
			ResetObjectState(cancel);
			JumpOffFromBed(cancel).Start();
		}

		IEnumerator<float> JumpToBed() {
			Player.anim.Play("PrepareToSleep");
			Player.anim.SetBool("Sleeping", true);
			PlayerAnimationHelper.StopPlayer();
			SpriteControler.Instance.FaceDOWN();

			yield return 0f;

			PlayerCommands.LastPositionBeforeJump = GameLibOfMethods.player.transform.position;
            //SpriteControler.Instance.ChangeSortingOrder(1);
            GameLibOfMethods.player.transform.parent.transform.parent = transform;
			while (true) {
				var state = Player.anim.GetCurrentAnimatorStateInfo(0);
				if (!state.IsName("PrepareToSleep")) {
					Debug.Log("Something is wrong.. this finished before firing the method. Make sure you don't call this from FixedUpdate.");
					break;
				}

				if (Player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 62 / 120f) { break; }

				yield return 0f;
			}


			Player.col.enabled = false;


			Vector2 targetPos = useableFunctionality.PlayerStandPosition;
			Vector2 currentPos = GameLibOfMethods.player.transform.position;

			while (true) {
				currentPos = Vector2.MoveTowards(currentPos, targetPos, .8f * GameTime.Time.deltaTime);
				GameLibOfMethods.player.transform.position = currentPos;
				if (currentPos == targetPos) { break; }
				yield return 0f;
			}

			yield return 0f;

			if (Random.Range(0f, 1f) > 0.5f)
				PlayerBuff.Add<DeepSleep>();
			
			GameLibOfMethods.isSleeping = true;
			GameLibOfMethods.cantMove = true;
			HandlePlayerSprites(enable: false);
            StartCoroutine(SleepSpeedChangeDelay());
            //Covers.sortingOrder = 2;
            useState = ObjectUseState.InUse;
			CurrentAction.BeginAction();
		}

        IEnumerator<WaitForSeconds> SleepSpeedChangeDelay()
        {
            yield return new WaitForSeconds(5);
            GameClock.ChangeSpeed(Player.anim.GetBool("Sleeping") == true ? 2 : 1);
            yield return new WaitForSeconds(5);
            GameClock.ChangeSpeed(Player.anim.GetBool("Sleeping") == true ? 5 : 1);
            yield return new WaitForSeconds(5);
            GameClock.ChangeSpeed(Player.anim.GetBool("Sleeping") == true ? 10 : 1);
        }

		IEnumerator<float> JumpOffFromBed(bool cancel) {
			Player.anim.SetBool("Sleeping", false);
			yield return 0f;

			while (true) {
				var state = Player.anim.GetCurrentAnimatorStateInfo(0);
				if (!state.IsName("JumpOffToBed")) {
					Debug.Log("Something is wrong.. this finished before firing the method. Make sure you don't call this from FixedUpdate.");
					break;
				}

				if (state.normalizedTime >= 40 / 50f) {
					break;
				}
				yield return 0f;
			}

			//Covers.sortingOrder = 0;
			HandlePlayerSprites(enable: true);

			void action() {
				Player.col.enabled = true;
                //SpriteControler.Instance.ChangeSortingOrder(0);
                //Covers.sortingOrder = 0;
                GameLibOfMethods.player.transform.parent.transform.parent = null;
                PlayerBuff.Remove<DeepSleep>();
            }
			PlayerCommands.JumpOff(useableFunctionality.CustomSpeedToPosition, action);
			//try { FinishUsing(cancel); }
			//catch { Debug.Log("TODO: Fix error on bed after animations btw."); }
		}

		void HandlePlayerSprites(bool enable) {
			SpriteControler.Instance.References[CharacterData.CharacterPart.Body].enabled = enable;
			SpriteControler.Instance.SetClothesState(enable);

			SpriteControler.Instance.Hand_L.enabled = enable;
			SpriteControler.Instance.Hand_R.enabled = enable;
		}

	}
}