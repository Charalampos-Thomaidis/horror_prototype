using UnityEngine;

public class Medkit : MonoBehaviour
{
    [SerializeField]
    private float restoreHealth;
    [SerializeField]
    private GameObject medkit;

    private PlayerHealth playerHealth;
    private Inventory inventory;

    void Start()
    {
        playerHealth = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        inventory = Inventory.Instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Use"))
        {
            UseMedkit();
        }
    }

    private void UseMedkit()
    {
        if (playerHealth != null && inventory != null)
        {
            if (inventory != null && inventory.HasItem(medkit) && medkit.activeInHierarchy && playerHealth.health < playerHealth.maxHealth)
            {
                playerHealth.RestoreHealth(restoreHealth);
                inventory.RemoveItem(medkit, true);
            }
        }
    }
}
