using System.Collections;
using UnityEngine;

public class PickUp : Interactable
{
    [SerializeField]
    private GameObject Item;

    private Inventory inventory;
    private string originalpromptMessage;

    void Start()
    {
        originalpromptMessage = promptMessege;

        inventory = Inventory.Instance;
    }
    protected override void Interact()
    {
        if (inventory != null)
        {
            if (inventory.IsInventoryFull())
            {
                StartCoroutine(ShowInventoryFullMessage());
            }
            else
            {
                AudioManager.Instance.PlayPickupSound();

                if (Item.name != "Flashlight")
                {
                    inventory.AddItem(Item);
                }
            }
        }
    }

    public void DestroyItem()
    {
        Destroy(Item);
    }

    private IEnumerator ShowInventoryFullMessage ()
    {
        promptMessege = "Full Inventory";
        yield return new WaitForSeconds(2f);
        promptMessege = originalpromptMessage;
    }
}