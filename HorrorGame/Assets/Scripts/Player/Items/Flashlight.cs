using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Flashlight : MonoBehaviour
{
    public bool on;
    public bool off;
    public float battery;
    public float maxBattery = 100f;
    public float startingBattery = 0.6f;
    public float consumeBattery = 1f;
    public Image BatteryEnergyBar;
    public TextMeshProUGUI batteryText;
    public GameObject spotlight;
    public Coroutine batteryDrainCoroutine;

    void Start()
    {
        battery = maxBattery * startingBattery;
        TurnOnFlashlight();
    }

    void Update()
    {
        battery = Mathf.Clamp(battery, 0, maxBattery);
        UpdateBatteryUI();

        if (off && Input.GetButtonDown("Flashlight"))
        {
            TurnOnFlashlight();
            AudioManager.Instance.PlayLightswitchSound();
        }
        else if (on && Input.GetButtonDown("Flashlight"))
        {
            TurnOffFlashlight();
            AudioManager.Instance.PlayLightswitchSound();
        }

        if (battery == 0)
        {
            TurnOffFlashlight();
        }
    }

    private void TurnOnFlashlight()
    {
        if (battery > 0)
        {
            spotlight.GetComponent<Light>().enabled = true;
            off = false;
            on = true;

            if (batteryDrainCoroutine != null)
            {
                StopCoroutine(batteryDrainCoroutine);
            }
            batteryDrainCoroutine = StartCoroutine(DrainBattery());
        }
    }

    private void TurnOffFlashlight()
    {
        battery -= 0.05f;
        spotlight.GetComponent<Light>().enabled = false;
        off = true;
        on = false;

        if (batteryDrainCoroutine != null)
        {
            StopCoroutine(batteryDrainCoroutine);
        }
    }

    public IEnumerator DrainBattery()
    {
        while (on && battery > 0)
        {
            yield return new WaitForSecondsRealtime(5f);

            if (!PauseMenu.GameIsPaused)
            {
                battery -= consumeBattery;
                UpdateBatteryUI();

                if (battery <= 0)
                {
                    TurnOffFlashlight();
                }
            }
        }
    }

    public void UpdateBatteryUI()
    {
        float fill = battery / maxBattery;
        BatteryEnergyBar.fillAmount = fill;
        batteryText.text = Mathf.RoundToInt(fill * 100) + "%";
    }

    public void RestoreBattery(float restore)
    {
        AudioManager.Instance.PlayChargingFlashlightSound();
        battery += restore;
        battery = Mathf.Clamp(battery, 0, maxBattery);
    }
}