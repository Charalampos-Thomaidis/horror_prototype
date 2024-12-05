using System.Collections;
using UnityEngine;

public class Door : Interactable
{
    public AudioClip OpenDoorSound;
    public AudioClip CloseDoorSound;

    [SerializeField]
    private GameObject door;
    [SerializeField]
    private bool keyDoor;
    [SerializeField]
    private bool keypadDoor;
    [SerializeField]
    private bool buttonDoor;
    [SerializeField]
    private GameObject key;

    public string code = "";

    private bool doorOpen;
    private string originalPromptMessage;
    private float interactionCooldown = 0.5f;
    private float lastInteractionTime = -1f;

    private Inventory inventory;
    private Animator doorAnim;
    private AudioSource audioSource;

    void Start()
    {
        originalPromptMessage = promptMessege;
        inventory = Inventory.Instance;
        doorAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void Interact()
    {
        if (keyDoor)
        {
            if (inventory != null && inventory.HasItem(key) && key.activeSelf)
            {
                UnlockDoor();
                inventory.RemoveItem(key, true);
            }
            else
            {
                LockedDoorInteraction();
            }
        }
        else if (keypadDoor)
        {
            LockedDoorInteraction();
        }
        else if (buttonDoor)
        {
            LockedDoorInteraction();
        }
        else
        {
            DoorInteraction();
        }
    }

    public void DoorInteraction()
    {
        if (Time.time < lastInteractionTime + interactionCooldown)
        {
            return;
        }

        lastInteractionTime = Time.time;
        doorOpen = !doorOpen;
        doorAnim.SetBool("isOpen", doorOpen);

        if (doorAnim.GetBool("isOpen"))
        {
            audioSource.clip = OpenDoorSound;
            audioSource.Play();
        }
        else
        {
            audioSource.clip = CloseDoorSound;
            audioSource.Play();
        }
    }

    public void UnlockDoor()
    {
        AudioManager.Instance.PlayUnlockedSound();
        keyDoor = false;
    }

    public void UnlockKeypadDoor()
    {
        keypadDoor = false;
    }

    public void UnlobckButonDoor()
    {
        buttonDoor = false;
    }

    public void LockedDoorInteraction()
    {
        StartCoroutine(DisplayLockedMessage());
    }

    private IEnumerator DisplayLockedMessage()
    {
        AudioManager.Instance.PlayLockedSound();
        promptMessege = "Locked.";
        yield return new WaitForSeconds(2f);
        promptMessege = originalPromptMessage;
    }

    public void OpenDoor()
    {
        if (!doorOpen)
        {
            doorOpen = true;
            doorAnim.SetBool("isOpen", true);
            audioSource.clip = OpenDoorSound;
            audioSource.Play();
        }
    }

    public void CloseDoor()
    {
        if (doorOpen)
        {
            doorOpen = false;
            doorAnim.SetBool("isOpen", false);
            audioSource.clip = CloseDoorSound;
            audioSource.Play();
        }
    }

    public bool IsClosed()
    {
        return !doorOpen;
    }
}