using System.Collections;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;

    private bool isFading = false;

    private void Start()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void Update()
    {
        if (TrialEndMenu.trialEnded || PlayerHealth.PlayerDied && !isFading)
        {
            StartCoroutine(HandleFadeInAnimation());
        }
    }

    private IEnumerator HandleFadeInAnimation()
    {
        isFading = true;
        yield return new WaitForSecondsRealtime(3f);
        PlaFadeInAnimation();
    }

    public void PlaFadeInAnimation()
    {
        animator.SetBool("isFading", true);
    }

    public void PlayFadeOutAnimation ()
    {
        animator.SetBool("isFading", false);
    }
}