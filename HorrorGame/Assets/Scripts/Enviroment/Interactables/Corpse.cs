using UnityEngine;

public class Corpse : Interactable
{
    public GameObject key;
    public GameObject trap;
    public bool containsKey;

    private PlayerHealth playerHealth;
    private bool isInteracting;

    void Start()
    {
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        Invoke(nameof(CheckForKey), 0.1f);
    }

    private void CheckForKey()
    {
        Transform spawn = transform.Find("spawn");
        if (spawn != null && spawn.childCount > 0)
        {
            containsKey = spawn.GetChild(0).CompareTag("Key");
        }
        else
        {
            containsKey = false;
        }

        if (!containsKey)
        {
            trap.SetActive(true);
        }
    }

    protected override void Interact()
    {
        if (isInteracting)
        {
            isInteracting = false;
            AudioManager.Instance.StopCorpseSearchingSound();

            if (containsKey)
            {
                LootKey();
            }
            else
            {
                TakeDamage();
            }
        }
    }

    public void StartInteraction()
    {
        if (!isInteracting)
        {
            isInteracting = true;
            AudioManager.Instance.PlayCorpseSearchingSound();
        }
    }

    public void CancelInteraction()
    {
        if (isInteracting)
        {
            isInteracting = false;
            AudioManager.Instance.StopCorpseSearchingSound();
        }
    }

    private void LootKey()
    {
        AudioManager.Instance.PlayPickupSound();
        Inventory.Instance.AddItem(key);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
    }

    private void TakeDamage()
    {
        AudioManager.Instance.PlayElectricShockSound();
        playerHealth.TakeDamage(80);
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
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
