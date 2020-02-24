using UnityEngine;

public class ActionBarUpdatingState : StateMachineBehaviour
{
	public bool hideProgressBar = false;
	public string ProgressText;
	int progressBarID;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		// We want an empty bar for the empty progress updates.
		// This is because currently the progress bar is being used as the action bar.

		// TODO: Change with Action UI.
		float GetCurrentProgress() => hideProgressBar ? 1 : 0;
		progressBarID = ProgressBar.StartTracking(ProgressText, GetCurrentProgress);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		ProgressBar.HideUI(progressBarID);
	}
}