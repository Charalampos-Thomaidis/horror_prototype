using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool useEvents;

    [SerializeField]
    public string promptMessege;

    public virtual string OnLook() 
    {
        return promptMessege;
    }
    public void BaseInteract()
    {
        if (useEvents) 
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        Interact();
    }

    protected virtual void Interact()
    {

    }
}