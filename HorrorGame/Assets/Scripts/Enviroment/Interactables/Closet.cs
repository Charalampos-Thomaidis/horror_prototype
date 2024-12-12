using UnityEngine;

public class Closet : Interactable
{
    [SerializeField]
    private AudioClip OpenClosetSound;
    [SerializeField]
    private AudioClip CloseClosetSound;
    [SerializeField]
    private Collider childTriggerCollider;

    private bool hasPlayerInside;
    private bool openCloset;
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = -1f;
    private Animator closetAnim;
    private AudioSource audioSource;
    private PlayerController playerController;

    private void Start()
    {
        closetAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();

        UpdateTriggerState();
    }

    protected override void Interact()
    {
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            return;
        }

        lastInteractionTime = Time.time;
        openCloset = !openCloset;
        closetAnim.SetBool("isOpen", openCloset);

        if (closetAnim.GetBool("isOpen"))
        {
            audioSource.clip = OpenClosetSound;
            audioSource.Play();
        }
        else
        {
            audioSource.clip= CloseClosetSound;
            audioSource.Play();
        }

        UpdateTriggerState();

        if (playerController.IsPlayerInCloset)
        {
            if (openCloset && hasPlayerInside)
            {
                playerController.ExitCloset();
            }
            else
            {
                playerController.EnterCloset();
            }
        }
    }

    public void OnChildTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!openCloset)
            {
                hasPlayerInside = true;
                playerController.EnterCloset();
            }
        }
    }

    public void OnChildTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayerInside = false;
            playerController.ExitCloset();
        }
    }

    private void UpdateTriggerState()
    {
        if (childTriggerCollider != null)
        {
            childTriggerCollider.enabled = !openCloset;
        }
    }

    public void Open()
    {
        if (!openCloset)
        {
            openCloset = true;
            closetAnim.SetBool("isOpen", true);
            audioSource.clip = OpenClosetSound;
            audioSource.Play();
        }
    }

    public void Close()
    {
        if (openCloset)
        {
            openCloset = false;
            closetAnim.SetBool("isOpen", false);
            audioSource.clip = CloseClosetSound;
            audioSource.Play();
        }
    }

    public bool IsClosed()
    {
        return !openCloset;
    }

    public bool PlayerInside()
    {
        return hasPlayerInside;
    }
}