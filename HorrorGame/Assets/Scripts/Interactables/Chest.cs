using System.Collections;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField]
    private GameObject chest;
    [SerializeField]
    private bool lockedChest;
    [SerializeField]
    private GameObject key;

    private bool chestOpen;
    private string originalPromptMessage;

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

    void Start()
    {
        randomNumber = Random.Range(0, 8);
        originalPromptMessage = promptMessege;
        inventory = Inventory.Instance;
    }

    public void ChestInteraction()
    {
        chestOpen = true;
        chest.GetComponent<Animator>().SetBool("isOpen", chestOpen);

        if (chest.GetComponent<Animator>().GetBool("isOpen"))
        {
            AudioManager.Instance.PlayOpenDrawerSound();
        }

        SetLayerRecursively(chest, LayerMask.NameToLayer("Default"));

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
            if (inventory !=null && inventory.HasItem(key) && key.activeSelf)
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
            ChestInteraction();
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