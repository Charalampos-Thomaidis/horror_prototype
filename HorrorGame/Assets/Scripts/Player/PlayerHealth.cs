using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static bool PlayerDied = false;

    public float health;
    public float maxHealth = 100f;
    public GameObject trialFailedUI;

    private bool deathSoundPlayed = false;

    void Start()
    {
        health = maxHealth;
        deathSoundPlayed = false;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 30)
        {
            StartCoroutine(DamageEffect());
        }
        if (health <= 0 && !PlayerDied)
        {
            trialFailedUI.SetActive(true);
            PlayerDied = true;
            VignetteManager.Instance.ApplyDeathEffect();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health > 0)
        {
            AudioManager.Instance.PlayHurtSound();
        }
        else if (!deathSoundPlayed)
        {
            AudioManager.Instance.PlayHurtSound();
            deathSoundPlayed = true;
        }

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