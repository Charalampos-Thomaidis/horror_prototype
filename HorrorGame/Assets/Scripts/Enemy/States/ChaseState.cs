using UnityEngine;

public class ChaseState : BaseState
{
    private float runSpeed = 8f;
    private float attackCooldownTimer;
    private float attackRange = 3.1f;
    private float attackCooldown = 2f;
    private float stopDistance = 2f;
    private float lostChaseTimer;

    public override void Enter()
    {
        enemy.Agent.stoppingDistance = stopDistance;
        enemy.Animator.SetBool("isEating", false);
        enemy.StopEatingSound();
    }

    public override void Perform()
    {
        if (enemy.CanSeePlayer() || enemy.CanHearSound())
        {
            enemy.InteractWithClosets();

            enemy.Player.gameObject.GetComponent<PlayerController>().IsInChase = true;

            lostChaseTimer = 0f;

            // Calculate distance to the player
            float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);

            // Update destination and movement speed based on distance
            if (distanceToPlayer > attackRange)
            {
                enemy.Agent.speed = runSpeed;
                enemy.Agent.isStopped = false;
                enemy.Agent.SetDestination(enemy.Player.transform.position);
            }
            else
            {
                enemy.Agent.isStopped = true;

                // Ensure that the enemy respects the stopping distance
                Vector3 directionToPlayer = (enemy.Player.transform.position - enemy.transform.position).normalized;
                Vector3 targetPosition = enemy.Player.transform.position - directionToPlayer * stopDistance;

                // Set the target position only if it's outside the stopping distance range
                float currentDistance = Vector3.Distance(enemy.transform.position, enemy.Player.transform.position);
                if (currentDistance > stopDistance)
                {
                    enemy.Agent.SetDestination(targetPosition);
                }
                else
                {
                    enemy.Agent.SetDestination(enemy.transform.position);
                }

                if (attackCooldownTimer <= 0f)
                {
                    enemy.AttackPlayer();
                    attackCooldownTimer = attackCooldown;
                }
            }

            // Rotate towards the player
            Vector3 direction = enemy.Player.transform.position - enemy.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        else 
        {
            lostChaseTimer += Time.deltaTime;

            if (lostChaseTimer > 10)
            {
                stateMachine.ChangeState(new SearchState());
            }
        }

        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0f)
        {
            enemy.Animator.SetBool("isAttacking", false);
        }
    }

    public override void Exit()
    {
        enemy.Player.gameObject.GetComponent<PlayerController>().IsInChase = false;
    }
}