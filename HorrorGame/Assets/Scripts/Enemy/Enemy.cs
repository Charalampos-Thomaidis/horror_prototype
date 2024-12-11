using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private string currentState; // for debug purposes
    //[SerializeField]
    //private GameObject debugsphere; // for debug purposes

    [Range(0f, 1f)]
    public float runStepLengthen;
    public float stepInterval;
    public float sightDistance;
    public float fieldOfView;
    public float eyeHeight;
    public float closetSearchRadius;
    public float interactionRange;
    public float hitRange;
    public Path path;
    public AudioClip footstepSound;
    public AudioClip eatSound;
    public AudioClip flinchSound;
    public AudioClip attackSound;

    private AudioSource footstep_audioSource;
    private AudioSource eat_audioSource;
    private AudioSource flinch_audioSource;
    private AudioSource attack_audioSource;
    private StateMachine stateMachine;
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject player;
    private Vector3 lastKnownPosition;
    private float stepCycle;
    private float nextStep;
    private Door lastInteractedDoor = null;
    private Closet lastInteractedCloset = null;
    private PlayerHealth playerHealth;
    private bool wasEating;
    private float soundTimer = 0f;
    private float soundDuration = 3f;
    private float hearDistance;

    public NavMeshAgent Agent { get => agent; }
    public Animator Animator { get => anim; }
    public GameObject Player { get => player; }
    public bool IsInSearchState { get; set; }
    public Vector3 lastSoundPosition { get; set; }
    public float ClosetSearchRadius { get => closetSearchRadius; }
    public bool IsChasing => stateMachine.activeState is ChaseState;

    void Start()
    {
        player = GameManager.Instance.Player;
        playerHealth = player.GetComponent<PlayerHealth>();
        stateMachine = GetComponent<StateMachine>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 4)
        {
            footstep_audioSource = audioSources[0];
            eat_audioSource = audioSources[1];
            flinch_audioSource = audioSources[2];
            attack_audioSource = audioSources[3];
        }

        stateMachine.Initialise();

        // Initialize lastSoundPosition to an invalid value
        lastSoundPosition = Vector3.positiveInfinity;

        stepCycle = 0;
        nextStep = stepCycle / 2f;
    }

    void Update()
    {
        if (CanSeePlayer())
        {
            lastKnownPosition = player.transform.position;
        }

        if (CanHearSound())
        {
            lastKnownPosition = lastSoundPosition;
        }

        if (soundTimer > 0)
        {
            soundTimer -= Time.deltaTime;

            // Reset lastSoundPosition only when the timer fully expires
            if (soundTimer <= 0)
            {
                lastSoundPosition = Vector3.positiveInfinity;
            }
        }

        InteractWithDoors();
        UpdateAnimations();

        currentState = stateMachine.activeState.ToString(); // for debug purposes
        //debugsphere.transform.position = lastKnownPosition; // for debug purposes
    }

    private void FixedUpdate()
    {
        ProgressStepCycle(agent.velocity.magnitude);
    }

    public bool CanSeePlayer()
    {
        if (player != null)
        {
            // Check if distance from enemy to player is less than sight distance
            if (Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                // Calculate the direction vector from the enemy's eye position to the player's position
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight);
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward);

                // Check if the player is within the enemy's field of view
                if (angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView)
                {
                    // Cast a ray from the enemy's eye position towards the player's position
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();

                    if (Physics.Raycast(ray, out hitInfo, sightDistance))
                    {
                        if (hitInfo.transform.gameObject == player)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool CanHearSound()
    {
        // Skip hearing check if lastSoundPosition is invalid or timer is expired
        if (lastSoundPosition == Vector3.positiveInfinity || soundTimer <= 0)
        {
            return false;
        }

        // Calculate the distance between the enemy and the last sound position
        float distanceToSound = Vector3.Distance(transform.position, lastSoundPosition);
        return distanceToSound <= hearDistance;
    }

    public void InteractWithDoors()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Ray ray = new Ray(transform.position, forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, interactionRange))
        {
            if (hitInfo.collider.CompareTag("Interactable"))
            {
                Door door = hitInfo.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (door.IsClosed())
                    {
                        if (lastInteractedDoor != door)
                        {
                            anim.SetBool("isInteracting", true);
                            door.OpenDoor();
                            lastInteractedDoor = door;
                        }
                    }
                }
            }
        }

        // No door in front, check if we need to close the last interacted door
        if (lastInteractedDoor != null)
        {
            // Track the distance to the last interacted door
            float distanceToLastDoor = Vector3.Distance(transform.position, lastInteractedDoor.transform.position);

            if (distanceToLastDoor > interactionRange)
            {
                lastInteractedDoor.CloseDoor();
                lastInteractedDoor = null;
            }
        }
    }

    public void InteractWithClosets()
    {
        if (IsInSearchState && lastInteractedCloset != null)
        {
            if (!lastInteractedCloset.IsClosed())
            {
                lastInteractedCloset = null;
            }
            else
            {
                return;
            }
        }

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        float sphereRadius = 0.5f;
        RaycastHit hitInfo;

        if (Physics.SphereCast(origin, sphereRadius, direction, out hitInfo, interactionRange))
        {
            if (hitInfo.collider.CompareTag("Interactable"))
            {
                Closet closet = hitInfo.collider.GetComponent<Closet>();
                if (closet != null && closet.IsClosed())
                {
                    if (lastInteractedCloset != closet)
                    {
                        anim.SetBool("isInteracting", true);
                        closet.Open();
                        lastInteractedCloset = closet;

                        if (closet.PlayerInside())
                        {
                            stateMachine.ChangeState(new KillState());
                        }
                    }
                }
            }
        }
    }

    private void ProgressStepCycle(float speed)
    {
        if (agent.velocity.magnitude > 0)
        {
            // Update the step cycle based on speed and time
            stepCycle += (agent.velocity.magnitude + (speed * runStepLengthen)) * Time.fixedDeltaTime;
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

    // Called by scripts which gonna alert the enemy
    public void NotifyEnemy(Vector3 soundPosition, float soundMaxDistance)
    {
        lastSoundPosition = soundPosition;
        hearDistance = soundMaxDistance;
        soundTimer = soundDuration;
    }

    // Called by scripts which gonna make the enemy enter flinch state
    public void Flinch()
    {
        PlayFlinchSound();
        stateMachine.ChangeState(new FlinchState());
    }
    
    // Called by animation event at the end of the flinch animation
    public void EndFlinch()
    {
        anim.SetBool("isFlinched", false);
        agent.isStopped = false;
    }

    // Called by animation event at the end of interact animation
    public void EndInteract()
    {
        anim.SetBool("isInteracting", false);
    }

    // Called by animation event at the middle of the attack animation
    public void OnAttackHit()
    {
        // Check if the player is within attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= hitRange)
        {
            playerHealth.TakeDamage(35);
        }
    }

    // Called by animation event at the start of finisher animation
    public void OnKillPlayer()
    {
        playerHealth.TakeDamage(100);
    }

    public void AttackPlayer()
    {
        if (!anim.GetBool("finisher") && playerHealth.health > 0)
        {
            PlayAttackSound();
            anim.SetBool("isAttacking", true);
        }

        if (playerHealth.health <= 0)
        {
            stateMachine.ChangeState(new KillState());
        }
    }

    public void PlayAttackSound()
    {
        attack_audioSource.clip = attackSound;
        attack_audioSource.Stop();
        attack_audioSource.Play();
    }

    public void PlayFlinchSound()
    {
        if (!flinch_audioSource.isPlaying)
        {
            flinch_audioSource.clip = flinchSound;
            flinch_audioSource.Play();
        }
    }

    public void PlayEatingSound()
    {
        if (!eat_audioSource.isPlaying)
        {
            eat_audioSource.clip = eatSound;
            eat_audioSource.playOnAwake = false;
            eat_audioSource.spatialBlend = 1f;
            eat_audioSource.minDistance = 1f;
            eat_audioSource.maxDistance = 5f;
            eat_audioSource.Play();
            wasEating = true;
        }
    }

    public void StopEatingSound()
    {
        if (eat_audioSource.isPlaying)
        {
            eat_audioSource.Stop();
            wasEating = false;
        }
    }

    public void PauseSound()
    {
        if (eat_audioSource.isPlaying && wasEating)
        {
            eat_audioSource.Pause();
        }

        if (attack_audioSource.isPlaying)
        {
            attack_audioSource.Pause();
        }

        if (flinch_audioSource.isPlaying)
        {
            flinch_audioSource.Pause();
        }
    }

    public void ResumeSound()
    {
        attack_audioSource.UnPause();
        flinch_audioSource.UnPause();

        if (wasEating)
        {
            eat_audioSource.UnPause();
        }
    }
}