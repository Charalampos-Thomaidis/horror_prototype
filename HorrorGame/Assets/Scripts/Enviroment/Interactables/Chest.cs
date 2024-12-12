using System.Collections;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField]
    private bool lockedChest;
    [SerializeField]
    private GameObject key;

    private bool chestOpen;
    private string originalPromptMessage;
    private bool isInteracting;

    public GameObject drop1;
    public GameObject drop2;
    public GameObject drop3;
    public GameObject drop4;
    public GameObject drop5;
    public GameObject drop6;
    public GameObject drop7;
    public GameObject drop8;
    public int randomNumber;

    private Inventory inventory;
    private Animator animChest;

    void Start()
    {
        randomNumber = Random.Range(0, 8);
        originalPromptMessage = promptMessege;
        inventory = Inventory.Instance;
        animChest = GetComponent<Animator>();
    }

    public void ChestInteraction()
    {
        chestOpen = true;
        animChest.SetBool("isOpen", chestOpen);

        if (animChest.GetBool("isOpen"))
        {
            AudioManager.Instance.PlayOpenDrawerSound();
        }

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));

        if (randomNumber == 0)
        {
            drop1.SetActive(true);
        }
        if (randomNumber == 1)
        {
            drop2.SetActive(true);
        }
        if (randomNumber == 2)
        {
            drop3.SetActive(true);
        }
        if (randomNumber == 3)
        {
            drop4.SetActive(true);
        }
        if (randomNumber == 4)
        {
            drop5.SetActive(true);
        }
        if (randomNumber == 5)
        {
            drop6.SetActive(true);
        }
        if (randomNumber == 6)
        {
            drop7.SetActive(true);
        }
        if (randomNumber == 7)
        {
            drop8.SetActive(true);
        }
    }

    public void UnlockChest()
    {
        AudioManager.Instance.PlayUnlockedSound();
        lockedChest = false;
    }

    public void LockedChestInteraction()
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

    protected override void Interact()
    {
        if (chestOpen) return;

        if (lockedChest)
        {
            if (inventory != null && inventory.HasItem(key) && key.activeSelf)
            {
                UnlockChest();
                key.SetActive(false);
            }
            else
            {
                LockedChestInteraction();
            }
        }
        else
        {
            if (isInteracting)
            {
                isInteracting = false;
                AudioManager.Instance.StopSearchChestSound();
                ChestInteraction();
            }
        }
    }

    public void StartInteraction()
    {
        if (!isInteracting)
        {
            isInteracting = true;
            AudioManager.Instance.PlaySearchChestSound();
        }
    }

    public void CancelInteraction()
    {
        if (isInteracting)
        {
            isInteracting = false;
            AudioManager.Instance.StopSearchChestSound();
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}