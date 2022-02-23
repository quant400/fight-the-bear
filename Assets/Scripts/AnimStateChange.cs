using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStateChange : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.CompareTag("Player"))
        {
            animator.gameObject.GetComponent<FightController>().ResetAnim();
        }
        else if (animator.gameObject.CompareTag("Bear"))
        {
            animator.gameObject.GetComponent<BearController>().ResetAnim();
        }
    }
}
