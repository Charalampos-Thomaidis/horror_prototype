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
        if (PlayerHealth.PlayerDied && !isFading)
        {
            StartCoroutine(HandleFadeInAnimation());
        }
        else if (TrialEndMenu.trialEnded)
        {
            PlayFadeInAnimation();
        }
    }

    private IEnumerator HandleFadeInAnimation()
    {
        isFading = true;
        yield return new WaitForSecondsRealtime(3f);
        PlayFadeInAnimation();
    }

    public void PlayFadeInAnimation()
    {
        animator.SetBool("isFading", true);
    }

    public void PlayFadeOutAnimation ()
    {
        animator.SetBool("isFading", false);
    }
}