using UnityEngine;

public class Button : Interactable
{
    [SerializeField]
    private GameObject button;
    private bool isPressed;

    protected override void Interact()
    {
        isPressed = !isPressed;
        button.GetComponent<Animator>().SetBool("isPressed", isPressed);
        AudioManager.Instance.PlayLightswitchSound();
    }
}
