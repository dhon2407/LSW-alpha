using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleAutoDestructState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ThoughtBubbleSpawner.Instance.DestroyBubble();
    }
}
