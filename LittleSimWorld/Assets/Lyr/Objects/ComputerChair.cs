using LSW.Helpers;
using UI;
using UnityEngine;
using System;


namespace Objects {
	public class ComputerChair : UseableObject {

		[SerializeField] private Vector2 computerPositionOffset = new Vector2(0, 1480f);
		
		bool ShouldCancel => Input.GetKeyDown(KeyCode.E) || GameLibOfMethods.passedOut;

		private Vector2 Position => Camera.main.WorldToViewportPoint(transform.position.Offset(computerPositionOffset));

        ImenuInteraction currentPC;
        HomePCInteractions pcFunctions;

        private void Start()
        {
            pcFunctions = FindObjectOfType<HomePCInteractions>();
            currentPC = pcFunctions.gameObject.GetComponent<Interactivity>();
        }



        public override void Interact()
        {
            if (pcFunctions == null) pcFunctions = FindObjectOfType<HomePCInteractions>();
            if (currentPC == null) currentPC = pcFunctions.gameObject.GetComponent<Interactivity>();
            currentPC.OpenMenu();
        }

        public void TrueInteract(Action action)
        {
            var result = TrySetCurrentAction("Sit");
			if (result) {
				SpriteControler.Instance.ChangeSortingOrder(1);
				GetComponent<SpriteRenderer>().sortingOrder = 2;
                if (pcFunctions == null) pcFunctions = FindObjectOfType<HomePCInteractions>();
                PlayerCommands.MoveTo(this, () => { BeginUsing(); action(); });
            }
			else { result.PrintErrorMessage(); }
        }

        public override void Use() {
			CurrentAction.ApplyStatsOnUse();

			if (shouldCancel) { FinishUsing(true); }
			else if (!CurrentAction.ValidateStats()) {
				useState = ObjectUseState.Preparation;
				ExitAfterDelay(false);
			}
		}

		public override void BeginUsing() {
			SpriteControler.Instance.UpdateCharacterOrientation(useableFunctionality.characterOrientationOnUseBegin);
			GameLibOfMethods.cantMove = true;
            GameLibOfMethods.canInteract = false;
            GameLibOfMethods.doingSomething = true;

            CurrentAction.BeginAction();
			useState = ObjectUseState.InUse;
			
			GamePauseHandler.SubscribeCloseEvent(EscCloseEvent);
		}
		public override void FinishUsing(bool cancel) {

			ResetObjectState(cancel);
            if (pcFunctions == null) pcFunctions = FindObjectOfType<HomePCInteractions>();
            pcFunctions.FinishActions();

            void ResetAction() {
				SpriteControler.Instance.ChangeSortingOrder(0);
				GetComponent<SpriteRenderer>().sortingOrder = 0;
			}
            PlayerAnimationHelper.ResetAnimations();
            PlayerCommands.WalkBackToLastPosition(ResetAction);
			GamePauseHandler.UnSubscribeCloseEvent(EscCloseEvent);
		}

		//public void ActivateChoices() => HomeComputer.instance.Open(Position);
		//public void DisableChoices() => HomeComputer.instance.Close();

		private bool EscCloseEvent()
		{
			if (CurrentAction == null) return false;
			
			FinishUsing(true);
			return true;
		}
	}
}