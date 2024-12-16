using System.Collections;
using UnityEngine;

public class ElevatorPanel : Interactable
{
    public Animator elevator_anim;
    public bool elevatorIsPowered;

    private bool interacted = false;
    private string originalPromptMessage;

    protected override void Interact()
    {
        if (!interacted && elevatorIsPowered)
        {
            AudioManager.Instance.PlayElevatorSound();
            elevator_anim.SetTrigger("CloseElevator");
            interacted = true;
        }

        if (!elevatorIsPowered)
        {
            StartCoroutine(DisplayOutOfPower());
        }
    }

    private IEnumerator DisplayOutOfPower()
    {
        promptMessege = "Out of power.";
        yield return new WaitForSeconds(1f);
        promptMessege = originalPromptMessage;
    }
}
