using System.Collections;
using UnityEngine;

public class FlinchState : BaseState
{
    private bool isFlinching;

    public override void Enter()
    {
        isFlinching = true;
        enemy.Animator.SetBool("isAttacking", false);
        enemy.Animator.SetBool("isEating", false);
        enemy.Animator.SetBool("isFlinched", true);
        enemy.Agent.isStopped = true;
        enemy.StartCoroutine(HandleFlinchDuration());
    }

    public override void Perform()
    {

    }

    public override void Exit()
    {

    }

    private IEnumerator HandleFlinchDuration()
    {
        yield return new WaitForSeconds(5f);

        if (isFlinching)
        {
            Exit();
            stateMachine.ChangeState(new SearchState());
        }
    }
}