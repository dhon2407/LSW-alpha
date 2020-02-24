using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;

public class CarWaitingForThePlayerState : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        JobCar.Instance.CarReadyToInteract = true;
    }

   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(GameClock.Time > System.TimeSpan.FromHours(JobManager.Instance.CurrentJob.JobStartingTimeInHours).TotalSeconds + System.TimeSpan.FromMinutes(30).TotalSeconds)
        {
            JobCar.Instance.CarDriveFromHouseToLeft();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        JobCar.Instance.CarReadyToInteract = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
