using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stats = PlayerStats.Stats;
using static PlayerStats.Status.Type;
using GameClock = GameTime.Clock;

public class RunningState : StateMachineBehaviour
{
    public float EnergyDrain = 2f;
    public float RunningSpeed = 1.5f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float Multi = GameTime.Time.deltaTime * GameClock.TimeMultiplier / GameClock.Speed;
        Stats.Status(Energy).Add(-EnergyDrain * Multi);
        Stats.MoveSpeed = RunningSpeed;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Stats.MoveSpeed = 1f;
    }
}