using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStats.Status;
using Stats = PlayerStats.Stats;

public class PlayerHiddenState : StateMachineBehaviour
{
    private GameObject[] occluders;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        occluders = GameObject.FindGameObjectsWithTag("PlayerOccluder");
        ChangeOccludersState(false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!JobManager.Instance.isWorking)
            ChangeOccludersState(true);
    }

    private void ChangeOccludersState(bool state)
    {
        foreach (var occluder in occluders)
        {
            occluder.GetComponent<SpriteRenderer>().enabled = state;
        }
    }
}
