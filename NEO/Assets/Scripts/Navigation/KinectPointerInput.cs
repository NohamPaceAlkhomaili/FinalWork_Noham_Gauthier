using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KinectPointerInput : MonoBehaviour
{
    [Header("References")]
    public HandCursorUI handCursorUI;
    public Camera uiCamera;

    private PointerEventData pointerData;
    private GameObject lastHovered;
    private bool lastHandClosed = false;

    void Start()
    {
        pointerData = new PointerEventData(EventSystem.current);
    }

    public void UpdatePointer(Vector2 screenPos, bool handClosed)
    {
        if (pointerData == null) return;
        
        pointerData.position = screenPos;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        GameObject hovered = null;
        foreach (var r in results)
        {
            if (r.gameObject.GetComponent<Button>() != null || 
                r.gameObject.GetComponent<Toggle>() != null ||
                r.gameObject.GetComponent<Slider>() != null)
            {
                hovered = r.gameObject;
                break;
            }
        }

        if (hovered != lastHovered)
        {
            if (lastHovered != null)
                ExecuteEvents.Execute(lastHovered, pointerData, ExecuteEvents.pointerExitHandler);
            if (hovered != null)
                ExecuteEvents.Execute(hovered, pointerData, ExecuteEvents.pointerEnterHandler);
            lastHovered = hovered;
        }

        if (handClosed && !lastHandClosed && hovered != null)
        {
            ExecuteEvents.Execute(hovered, pointerData, ExecuteEvents.pointerDownHandler);
        }
        else if (!handClosed && lastHandClosed && hovered != null)
        {
            ExecuteEvents.Execute(hovered, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(hovered, pointerData, ExecuteEvents.pointerClickHandler);
        }

        lastHandClosed = handClosed;
    }
}
