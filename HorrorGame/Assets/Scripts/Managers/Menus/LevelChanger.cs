using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        yield return new WaitForSecondsRealtime(5f);
        TriggerFadeIn();
    }

    public void TriggerFadeIn()
    {
        animator.SetBool("isFading", true);
    }

    public void TriggerFadeOut ()
    {
        animator.SetBool("isFading", false);
    }
}
