
public class KillState : BaseState
{

    public override void Enter()
    {
        enemy.Agent.isStopped = true;
        enemy.Animator.SetBool("isAttacking", false);
        enemy.Animator.SetBool("finisher", true);
        enemy.PlayEatingSound();
    }

    public override void Perform()
    {

    }

    public override void Exit()
    {

    }
}
