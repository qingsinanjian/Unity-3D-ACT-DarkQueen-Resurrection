using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputStateController : StateMachineBehaviour
{
    public bool lockAll = true;
    public bool lockMove;
    public bool lockJump;
    public bool lockAttack;
    public bool lockEquip;
    public bool lockUseSkill;
    public bool onlyLock;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(lockAll)
            animator.GetComponent<PlayerController>().canGetPlayerInputValue = false;
        if(lockMove)
            animator.GetComponent<PlayerController>().canMove = false;
        if(lockJump)
            animator.GetComponent<PlayerController>().canJump = false;
        if(lockAttack)
            animator.GetComponent<PlayerController>().canAttack = false;
        if(lockEquip)
            animator.GetComponent<PlayerController>().canEquip = false;
        if(lockUseSkill)
            animator.GetComponent<PlayerController>().canUseSkill = false;
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onlyLock) return;
        if (lockAll)
            animator.GetComponent<PlayerController>().UnLockAll();
        if (lockMove)
            animator.GetComponent<PlayerController>().canMove = true;
        if (lockJump)
            animator.GetComponent<PlayerController>().canJump = true;
        if (lockAttack)
            animator.GetComponent<PlayerController>().canAttack = true;
        if (lockEquip)
            animator.GetComponent<PlayerController>().canEquip = true;
        if (lockUseSkill)
            animator.GetComponent<PlayerController>().canUseSkill = true;
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
