using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField]
    private GameObject energyDrink;
    [SerializeField]
    private float speedBuff;
    [SerializeField]
    private float duration = 5f;

    private PlayerController playerController;
    private Inventory inventory;
    private bool isBuffActive = false;

    public bool canUseItem = true;

    void Start()
    {
        playerController = GameManager.Instance.Player.GetComponent<PlayerController>();
        inventory = Inventory.Instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Use") && CanUseItem() && !TrialEndMenu.trialEnded && !DialogueManager.Instance.IsDialogueActive() && !PlayerHealth.PlayerDied && !PauseMenu.GameIsPaused)
        {
            UseSyringe();
        }
    }

    private bool CanUseItem()
    {
        return canUseItem && !playerController.IsPlayerInCloset;
    }

    private void UseSyringe()
    {
        if (inventory != null && inventory.HasItem(energyDrink) && energyDrink.activeSelf)
        {
            if (playerController != null && !isBuffActive && !BuffManager.Instance.IsBuffActive())
            {
                isBuffActive = true;
                AudioManager.Instance.PlayBuffSound();
                BuffManager.Instance.ApplyBuff(playerController, speedBuff, duration, Color.yellow);
                inventory.RemoveItem(energyDrink, true);
            }
        }
    }

    public void SpeedBuff()
    {
        isBuffActive = true;
        BuffManager.Instance.ApplyBuff(playerController, speedBuff, duration, Color.yellow);
    }
}