using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject crosshairImage;
    [SerializeField]
    private GameObject handImage;
    [SerializeField]
    private TextMeshProUGUI promptText;
    [SerializeField]
    private TextMeshProUGUI useItemText;
    [SerializeField]
    private TextMeshProUGUI dropItemText;
    [SerializeField]
    public GameObject dialogueUI;
    [SerializeField]
    public GameObject noteUI;
    [SerializeField]
    private GameObject inventoryUI;
    [SerializeField]
    private UnityEngine.UI.Button[] itemSlots;
    [SerializeField]
    private List<GameObject> activeItemSlots;

    private Dictionary<string, Button> itemToUIMap;

    void Start()
    {
        promptText.gameObject.SetActive(true);
        crosshairImage.SetActive(true);
        handImage.SetActive(false);

        itemToUIMap = new Dictionary<string, Button>
        {
            {"Key", FindButton("KeyImage") },
            {"ChestKey", FindButton("ChestKeyImage") },
            {"Battery", FindButton("BatteryImage") },
            {"Pill", FindButton("MedkitImage") },
            {"Syringe", FindButton("SyringeImage") },
            {"Bottle", FindButton("BottleImage") }
        };

        foreach (var uiElement in itemToUIMap.Values)
        {
            if (uiElement != null)
            {
                uiElement.gameObject.SetActive(false);
            }
        }

        foreach (UnityEngine.UI.Button button in itemSlots)
        {
            button.gameObject.SetActive(false);
        }

        inventoryUI.SetActive(false);
    }

    private Button FindButton(string gameObjectName)
    {
        GameObject obj = GameObject.Find(gameObjectName);
        if (obj != null)
        {
            return obj.GetComponent<Button>();
        }
        else
        {
            return null;
        }
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public void UpdateControlsText(GameObject heldItem)
    {
        if (heldItem != null)
        {
            string itemName = heldItem.name;

            if (itemToUIMap.TryGetValue(itemName, out Button button))
            {
                useItemText.gameObject.SetActive(true);
                dropItemText.gameObject.SetActive(true);

                if (itemName == "Bottle")
                {
                    useItemText.text = "Throw (M1)";
                    dropItemText.text = "Drop (G)";
                }
                else if (!(itemName == "Bottle"))
                {
                    useItemText.text = "Use (V)";
                    dropItemText.text = "Drop (G)";
                }
                if (itemName == "Key")
                {
                    useItemText.text = "";
                    dropItemText.text = "";
                }
            }
            else
            {
                useItemText.gameObject.SetActive(false);
                dropItemText.gameObject.SetActive(false);
            }
        }
        else
        {
            useItemText.gameObject.SetActive(false);
            dropItemText.gameObject.SetActive(false);
        }
    }

    public void SetCrosshairImageActive(bool isActive)
    {
        crosshairImage.SetActive(isActive);
        handImage.SetActive(!isActive);
    }

    public void ShowInventoryUI()
    {
        inventoryUI.SetActive(true);
    }

    public void HideInventoryUI()
    {
        inventoryUI.SetActive(false);
    }

    public void UpdateInventory(List<GameObject> items, GameObject currentlyHeldItem) 
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].GetComponentInChildren<TextMeshProUGUI>().text = items[i].name;
                itemSlots[i].gameObject.SetActive(true);
            }
            else
            {
                itemSlots[i].gameObject.SetActive(false);
            }
        }
        UpdateControlsText(currentlyHeldItem);
    }

    public void UpdateActiveItemsSlot(int activeItemIndex)
    {
        for (int i = 0; i < activeItemSlots.Count; i++)
        {
            if (activeItemSlots[i] != null)
            {
                activeItemSlots[i].SetActive(i == activeItemIndex);
            }
        }
    }

    public void DisableText()
    {
        dropItemText.gameObject.SetActive(false);
        useItemText.gameObject.SetActive(false);
        dialogueUI.SetActive(false);
        noteUI.SetActive(false);
    }
}