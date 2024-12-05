using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerInteractUI : MonoBehaviour
{
    public Camera cam;
    public float raycastDistance = 5f;
    public LayerMask layerMask;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
        {
            if (Input.GetButtonDown("Interact"))
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current)
                {
                    pointerId = -1,
                    position = Input.mousePosition
                };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("UI"))
                    {
                        ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                    }
                }
            }
        }
    }
}