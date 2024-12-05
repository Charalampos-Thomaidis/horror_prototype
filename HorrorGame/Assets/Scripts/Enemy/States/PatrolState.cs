using UnityEngine;

public class PatrolState : BaseState
{
    public int waypointIndex;
    public float waitTimer;
    private float walkSpeed = 5f;
    private bool hasStartedEating;

    public override void Enter()
    {
        enemy.Agent.speed = walkSpeed;
        enemy.Agent.stoppingDistance = 0.01f;
        enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
        waitTimer = 0f;
        hasStartedEating = false;
        enemy.Animator.SetBool("isEating", false);
        enemy.StopEatingSound();
    }

    public override void Perform()
    {
        // Check if the enemy has reached the destination
        if (enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance)
        {
            if (!hasStartedEating)
            {
                enemy.Animator.SetBool("isEating", true);
                enemy.PlayEatingSound();
                hasStartedEating = true;
            }

            waitTimer += Time.deltaTime;
            if (waitTimer > 10)
            {
                enemy.Animator.SetBool("isEating", false);
                enemy.StopEatingSound();

                // Move to the next waypoint if the wait timer exceeds 5 seconds
                if (waypointIndex < enemy.path.waypoints.Count - 1)
                {
                    waypointIndex++;
                }
                else
                {
                    waypointIndex = 0;
                }

                // Set the destination to the next waypoint in the patrol path
                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
                hasStartedEating = false;
            }
        }

        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new ChaseState());
        }

        if (enemy.CanHearSound())
        {
            stateMachine.ChangeState(new SearchState());
            Vector3 soundPosition = enemy.lastSoundPosition;
            enemy.Agent.SetDestination(soundPosition);
        }
    }

    public override void Exit()
    {

    }
}