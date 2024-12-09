using UnityEngine;
using UnityEngine.AI;

public class MenuPatrol : MonoBehaviour
{
    public float waitTime = 10f;
    public float walkSpeed = 5f;
    private float waitTimer = 0f;
    private bool hasStartedEating = false;
    private int waypointIndex = 0;
    public float stepInterval;
    private float stepCycle;
    private float nextStep;

    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource footstep_audioSource;
    private AudioSource eat_audioSource;
    public AudioClip footstepSound;
    public AudioClip eatingSound;
    public Path path;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            footstep_audioSource = audioSources[0];
            eat_audioSource = audioSources[1];
        }

        agent.speed = walkSpeed;
        agent.stoppingDistance = 0.01f;
        SetNextWaypoint();
    }

    void Update()
    {
        PatrolAndEat();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ProgressStepCycle(agent.velocity.magnitude);
    }

    private void ProgressStepCycle(float speed)
    {
        if (agent.velocity.magnitude > 0)
        {
            // Update the step cycle based on speed and time
            stepCycle += (agent.velocity.magnitude + speed) * Time.fixedDeltaTime;
        }

        if (stepCycle > nextStep)
        {
            // Set the time for the next step
            nextStep = stepCycle + stepInterval;
            footstep_audioSource.clip = footstepSound;
            footstep_audioSource.PlayOneShot(footstepSound);
        }
    }

    // Set the animation from idle => walk => run according to the enemy speed velocity
    private void UpdateAnimations()
    {
        float speed = agent.velocity.magnitude;
        anim.SetFloat("speed", speed);
    }

    private void PatrolAndEat()
    {
        // Check if we've reached the waypoint
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            if (!hasStartedEating)
            {
                anim.SetBool("isEating", true);
                PlayEatingSound();
            }

            waitTimer += Time.deltaTime;
            if (waitTimer > waitTime)
            {
                anim.SetBool("isEating", false);
                StopEatingSound();

                // Move to the next waypoint if the wait timer exceeds the threshold
                waypointIndex = (waypointIndex + 1) % path.waypoints.Count;
                SetNextWaypoint();
                waitTimer = 0f;
                hasStartedEating = false;
            }
        }
    }

    private void SetNextWaypoint()
    {
        agent.SetDestination(path.waypoints[waypointIndex].position);
    }

    public void PlayEatingSound()
    {
        if (!eat_audioSource.isPlaying)
        {
            eat_audioSource.clip = eatingSound;
            eat_audioSource.playOnAwake = false;
            eat_audioSource.spatialBlend = 1f;
            eat_audioSource.minDistance = 1f;
            eat_audioSource.maxDistance = 10f;
            eat_audioSource.Play();
            hasStartedEating = true;
        }
    }

    public void StopEatingSound()
    {
        if (eat_audioSource.isPlaying)
        {
            eat_audioSource.Stop();
            hasStartedEating = false;
        }
    }
}