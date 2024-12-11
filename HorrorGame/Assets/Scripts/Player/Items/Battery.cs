using UnityEngine;

public class Battery : MonoBehaviour
{
    [SerializeField]
    private float restoreBattery;
    [SerializeField]
    private GameObject battery;

    private Flashlight flashlight;
    private Inventory inventory;

    void Start()
    {
        flashlight = GameManager.Instance.FlashlightHolder.GetComponent<Flashlight>();
        inventory = Inventory.Instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Use") && !TrialEndMenu.trialEnded && !DialogueManager.Instance.IsDialogueActive() && !PlayerHealth.PlayerDied && !PauseMenu.GameIsPaused)
        {
            UseBattery();
        }
    }

    private void UseBattery()
    {
        if (flashlight != null && inventory != null)
        {
            if (battery != null && inventory.HasItem(battery) && battery.activeInHierarchy)
            {
                if (flashlight.isActiveAndEnabled && flashlight.battery < flashlight.maxBattery)
                {
                    flashlight.RestoreBattery(restoreBattery);
                    inventory.RemoveItem(battery, true);
                }
            }
        }
    }
}
