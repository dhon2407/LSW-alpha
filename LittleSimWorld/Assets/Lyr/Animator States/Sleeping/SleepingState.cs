using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using GameClock = GameTime.Clock;
using Stats = PlayerStats.Stats;
using static PlayerStats.Status.Type;

public class SleepingState : StateMachineBehaviour {

	public int EnergyGainPerSecond = 30;
	public int HealthReducePerSecond = 10;
	public int EnergyRequiredToWakeUp = 50;
	public int TimeMultiplierWhileSleeping = 10;
	public bool Cancelable;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameClock.ChangeSpeed(TimeMultiplierWhileSleeping);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		float Multi = GameTime.Time.deltaTime * GameClock.TimeMultiplier / GameClock.Speed;

        Stats.Status(Energy).Add(EnergyGainPerSecond * Multi);

		bool ShouldExitState = Stats.Status(Energy).CurrentAmount >= EnergyRequiredToWakeUp || (Cancelable && Input.GetKeyDown(KeyCode.E));
		if (ShouldExitState) { animator.SetBool("Sleeping", false); }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GameTime.Clock.ResetSpeed();
		PlayerAnimationHelper.ResetPlayer();
	}

}
