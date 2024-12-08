using UnityEngine;

public class Button : Interactable
{
    private bool isPressed;
    private Animator animButton;

    private void Start()
    {
        animButton = GetComponent<Animator>();
    }

    protected override void Interact()
    {
        isPressed = !isPressed;
        animButton.SetBool("isPressed", isPressed);
        AudioManager.Instance.PlayLightswitchSound();
    }
}
