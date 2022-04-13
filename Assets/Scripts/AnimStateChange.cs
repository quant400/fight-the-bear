using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStateChange : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.CompareTag("Player"))
        {
            FightController fc = animator.gameObject.GetComponent<FightController>();
            fc.actionDone = false;
            if (fc.GetState() == States.Dead)
            {
                fc.Die();
                return;
            }

        }
        else if (animator.gameObject.CompareTag("Bear"))
        {
            BearController bc = animator.gameObject.GetComponent<BearController>();
            if (bc.GetState() == States.Dead)
            {
                bc.StopTimer();
                bc.Invoke("Die",2f);
                return;
            }
           
            bc.actionDone = false;
           

        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.CompareTag("Player"))
        {
            FightController fc = animator.gameObject.GetComponent<FightController>();
            fc.actionDone = true;
           
            if (fc.GetSpecialAttackStatus() && fc.GetState()!=States.Hit)
            {
                fc.DisableSpecialAttack();
                return;
            }
            fc.ResetAnim();
           if(fc.GetBear().GetState() != States.Dead && fc.GetState()!=States.Dead)
                fc.PlayActions(); 


        }
        else if (animator.gameObject.CompareTag("Bear"))
        {
            FightController fc = GameObject.FindGameObjectWithTag("Player").GetComponent<FightController>();
            BearController bc = animator.gameObject.GetComponent<BearController>();
            bc.actionDone = true;
            bc.ResetAnim();
            if (fc.actionDone && !fc.GetSpecialAttackStatus())
                fc.PlayActions();
           

        }
    }
}
