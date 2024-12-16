using System.Collections.Generic;
using UnityEngine;

public class SearchState : BaseState
{
    private float searchTimer;
    private float moveTimer;
    private float rotationSpeed = 5f;
    private float runSpeed = 6.5f;
    private float stopDistanceFromCloset = 1.5f;
    private Closet targetCloset;
    private Transform targetSearchPoint;
    private bool hasSearchedCloset;
    private bool canInteractWithCloset;

    public override void Enter()
    {
        enemy.Agent.speed = runSpeed;
        enemy.Animator.SetBool("isEating", false);
        enemy.StopEatingSound();
        targetCloset = null;
        targetSearchPoint = null;
        hasSearchedCloset = false;
        canInteractWithCloset = true;
        enemy.IsInSearchState = true;
    }

    public override void Perform()
    {
        if (canInteractWithCloset)
        {
            enemy.InteractWithClosets();
        }

        if (enemy.CanSeePlayer())
        {
            stateMachine.ChangeState(new ChaseState());
            return;
        }

        if (enemy.CanHearSound())
        {
            Vector3 soundPosition = enemy.lastSoundPosition;
            enemy.Agent.SetDestination(soundPosition);
            canInteractWithCloset = true;
        }

        // Check if the enemy has reached its destination
        if (enemy.Agent.remainingDistance < enemy.Agent.stoppingDistance)
        {
            searchTimer += Time.deltaTime;
            moveTimer += Time.deltaTime;

            // Search for closets if none is currently being targeted
            if (!hasSearchedCloset && targetCloset == null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, enemy.ClosetSearchRadius);
                var closets = new List<Closet>();

                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Interactable"))
                    {
                        Closet closet = hitCollider.GetComponent<Closet>();
                        if (closet != null && closet.IsClosed())
                        {
                            closets.Add(closet);
                        }
                    }
                }

                if (closets.Count > 0)
                {
                    targetCloset = closets[Random.Range(0, closets.Count)];
                    enemy.Agent.speed = 5f;
                    targetSearchPoint = targetCloset.transform.Find("SearchPoint");

                    if (targetSearchPoint != null)
                    {
                        // Set the destination to the search point inside the closet
                        Vector3 directionToSearchPoint = (targetSearchPoint.position - enemy.transform.position).normalized;
                        Vector3 stoppingPosition = targetSearchPoint.position - directionToSearchPoint * stopDistanceFromCloset;
                        enemy.Agent.SetDestination(stoppingPosition);
                    }
                }
            }

            else if (targetCloset != null && targetSearchPoint != null)
            {

                if (Vector3.Distance(enemy.transform.position, targetSearchPoint.position) <= enemy.interactionRange)
                {
                    // Calculate direction to closet without affecting the y-axis
                    Vector3 directionToCloset = (targetSearchPoint.position - enemy.transform.position);
                    directionToCloset.y = 0;

                    Quaternion targetRotation = Quaternion.LookRotation(directionToCloset);
                    enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    // Check if rotation is close enough to complete
                    float angleDifference = Quaternion.Angle(enemy.transform.rotation, targetRotation);
                    if (angleDifference < 5f)
                    {
                        targetCloset = null;
                        targetSearchPoint = null;
                        hasSearchedCloset = true;
                        moveTimer = 0f;
                    }
                }
            }

            else if (hasSearchedCloset)
            {
                // Move to a new random position after checking closet
                if (moveTimer > 1f)
                {
                    enemy.Agent.speed = runSpeed;
                    enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 15));
                    moveTimer = 0f;
                    canInteractWithCloset = false;
                }
            }
        }

        if (searchTimer > 5f)
        {
            enemy.IsInSearchState = false;
            stateMachine.ChangeState(new PatrolState());
        }
    }

    public override void Exit()
    {
        targetCloset = null;
        targetSearchPoint = null;
        hasSearchedCloset = false;
        enemy.IsInSearchState = false;
        canInteractWithCloset = false;
    }
}