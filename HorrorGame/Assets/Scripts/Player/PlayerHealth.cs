using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static bool PlayerDied = false;

    public float health;
    public float maxHealth = 100f;
    public GameObject dialogueUI;
    public GameObject noteUI;
    public GameObject trialFailUI;

    private PlayerController playerController;
    private PlayerInteract playerInteract;
    private PlayerUI playerUI;
    private Flashlight flashlight;
    private GameObject inventory;

    void Start()
    {
        health = maxHealth;
        playerController = GetComponent<PlayerController>();
        playerInteract = GetComponent<PlayerInteract>();
        playerUI = GetComponent<PlayerUI>();
        flashlight = GameManager.Instance.FlashlightHolder.GetComponent<Flashlight>();
        inventory = GameManager.Instance.Inventory;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 30)
        {
            StartCoroutine(DamageEffect());
        }
        if (health <= 0)
        {
            PlayerDied = true;
            VignetteManager.Instance.ApplyDeathEffect();
            playerController.enabled = false;
            playerInteract.enabled = false;
            playerUI.DisableText();
            flashlight.enabled = false;
            playerController.mouseLook.SetCursorLock(false);
            trialFailUI.SetActive(true);
            inventory.SetActive(false);
            dialogueUI.SetActive(false);
            noteUI.SetActive(false);
        }
    }

    public void TakeDamage(float damage)
    {
        AudioManager.Instance.PlayHurtSound();
        health -= damage;
        StartCoroutine(DamageEffect());
    }

    IEnumerator DamageEffect()
    {
        VignetteManager.Instance.ApplyVignetteEffect(Color.red, 0.4f, 0.4f, VignetteManager.VignetteState.Damage);
        yield return new WaitForSeconds(0.4f);
    }

    public void RestoreHealth(float healthAmount)
    {
        AudioManager.Instance.PlayHealSound();
        health += healthAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        StartCoroutine(HealEffect());
    }

    IEnumerator HealEffect()
    {
        VignetteManager.Instance.ApplyVignetteEffect(Color.green, 0.4f, 1f, VignetteManager.VignetteState.Heal);
        yield return new WaitForSeconds(1f);
    }
}