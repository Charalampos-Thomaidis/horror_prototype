using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance {  get; private set; }
    private Transform originalParent;

    public List<GameObject> items = new List<GameObject>();
    public Transform inventoryTransform;
    public LayerMask groundLayer;
    public int MaxItems = 6;

    private PlayerUI playerUI;
    private GameObject currentlyHeldItem = null;
    private int currentlyHeldItemIndex = -1;
    private bool isInventoryOpen = false;

    public bool canDropItem = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            originalParent = transform.parent;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
        }

        playerUI = GameManager.Instance.Player.GetComponent<PlayerUI>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Drop") && CanDropItem())
        {
            DropCurrentItem();
        }

        if (Input.GetButtonDown("Switch"))
        {
            SwitchHeldItem();
        }

        if (Input.GetButtonDown("Inventory"))
        {
            if (isInventoryOpen)
            {
                playerUI.HideInventoryUI();
                isInventoryOpen = false;
            }
            else
            {
                playerUI.ShowInventoryUI();
                isInventoryOpen = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectItem(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectItem(5);

        UpdateItemPosition();
    }

    public void AddItem(GameObject item)
    {
        if (CountItems() >= MaxItems)
        {
            return;
        }

        items.Add(item);
        item.transform.SetParent(inventoryTransform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (currentlyHeldItem == null)
        {
            currentlyHeldItem = item;
            currentlyHeldItemIndex = items.IndexOf(item);
            PositionItemInFrontOfCamera(item);
            item.SetActive(true);
        }
        else
        {
            currentlyHeldItem.SetActive(false);
            item.SetActive(true);
            currentlyHeldItem = item;
            currentlyHeldItemIndex = items.IndexOf(item);
            PositionItemInFrontOfCamera(item);
        }

        SetLayerRecursively(item, LayerMask.NameToLayer("HeldItem"));
        playerUI.UpdateInventory(items, currentlyHeldItem);
        playerUI.UpdateActiveItemsSlot(currentlyHeldItemIndex);
    }

    public void RemoveItem(GameObject item, bool destroyItem)
    {
        if (items.Contains(item))
        {
            if (destroyItem)
            {
                items.Remove(item);
                Destroy(item);
            }
            else
            {
                items.Remove(item);
            }

            if (item == currentlyHeldItem)
            {
                SwitchToFirstItemOnDrop();
            }

            playerUI.UpdateInventory(items, currentlyHeldItem);
        }
    }

    public bool HasItem(GameObject item)
    {
        return items.Contains(item);
    }
    
    private int CountItems()
    {
        return items.Count;
    }

    public bool IsInventoryFull()
    {
        return CountItems() >= MaxItems; 
    }
    
    private bool CanDropItem()
    {
        return canDropItem && currentlyHeldItem != null && currentlyHeldItem.name != "Key";
    }

    private void DropCurrentItem()
    {
        if (currentlyHeldItem == null)
        {
            return;
        }

        currentlyHeldItem.transform.SetParent(null);

        RaycastHit hit;
        Vector3 dropPosition = transform.position;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            dropPosition = hit.point;
        }

        dropPosition += new Vector3(0, 0.1f, 0);

        currentlyHeldItem.transform.position = dropPosition;
        currentlyHeldItem.transform.rotation = Quaternion.Euler(0, 90, 0);
        SetLayerRecursively(currentlyHeldItem, LayerMask.NameToLayer("Interactable"));

        RemoveItem(currentlyHeldItem, false);
        playerUI.UpdateInventory(items, currentlyHeldItem);
    }

    private void SelectItem(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    item.SetActive(false);
                }
            }

            currentlyHeldItemIndex = index;
            currentlyHeldItem = items[currentlyHeldItemIndex];

            if (currentlyHeldItem != null)
            {
                currentlyHeldItem.SetActive(true);
                PositionItemInFrontOfCamera(currentlyHeldItem);
            }

            playerUI.UpdateActiveItemsSlot(currentlyHeldItemIndex);
            playerUI.UpdateInventory(items, currentlyHeldItem);
        }
    }

    private void SwitchHeldItem()
    {
        items.RemoveAll(item => item == null);

        if (items.Count == 0)
        {
            currentlyHeldItem = null;
            currentlyHeldItemIndex = -1;
            playerUI.UpdateActiveItemsSlot(-1);
            return;
        }

        foreach (var item in items)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }

        currentlyHeldItemIndex = (currentlyHeldItemIndex + 1) % items.Count;
        currentlyHeldItem = items[currentlyHeldItemIndex];

        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.SetActive(true);
            PositionItemInFrontOfCamera(currentlyHeldItem);
        }

        playerUI.UpdateActiveItemsSlot(currentlyHeldItemIndex);
        playerUI.UpdateInventory(items, currentlyHeldItem);
    }

    private void SwitchToFirstItemOnDrop()
    {
        items.RemoveAll(item => item == null);

        if (items.Count == 0)
        {
            currentlyHeldItem = null;
            currentlyHeldItemIndex = -1;

            playerUI.UpdateActiveItemsSlot(-1);
            return;
        }

        foreach (var item in items)
        {
            if (item != null)
            {
                item.SetActive(false);
            }
        }

        currentlyHeldItemIndex = 0;
        currentlyHeldItem = items[currentlyHeldItemIndex];

        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.SetActive(true);
            PositionItemInFrontOfCamera(currentlyHeldItem);
        }

        playerUI.UpdateActiveItemsSlot(currentlyHeldItemIndex);
    }

    private void PositionItemInFrontOfCamera(GameObject item)
    {
        item.transform.SetParent(inventoryTransform);
        item.transform.localPosition = new Vector3(0.6f, -0.5f, 0.5f);
        item.transform.localRotation = Quaternion.identity;
        item.SetActive(true);
    }

    private void UpdateItemPosition()
    {
        if (currentlyHeldItem != null)
        {
            currentlyHeldItem.transform.localPosition = new Vector3(0.6f, -0.5f, 0.5f);
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