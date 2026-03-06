using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TouchpadController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [System.Serializable]
    public class Vector2Event : UnityEvent<Vector2> { }

    public Vector2Event onDragDelta;  

    private Vector2 pointerDownPosition;
    private Vector2 currentPosition;
    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownPosition = eventData.position;
        currentPosition = pointerDownPosition;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        currentPosition = eventData.position;
        Vector2 delta = currentPosition - pointerDownPosition;
        delta /= Screen.dpi;  

       
        onDragDelta?.Invoke(delta);

        
        pointerDownPosition = currentPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        onDragDelta?.Invoke(Vector2.zero);  
    }
}