using UnityEngine;

public class ItemSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField]
    private float smooth;
    [SerializeField]
    private float swayMultiplier;


    void Update()
    {
        if (!TrialEndMenu.trialEnded && !DialogueManager.Instance.IsDialogueActive() && !PlayerHealth.PlayerDied && !PauseMenu.GameIsPaused)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }
    }
}
