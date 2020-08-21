using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableFigure : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        DragAndDropController.Instance.StartDrag(this);
        transform.SetAsLastSibling(); // this will move UI-object you click on to the "front" of your UI by putting it at the "end" of your hierarchy
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragAndDropController.Instance.EndDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragAndDropController.Instance.IsDragging())
        {
            transform.position = Input.mousePosition;
        }
    }
}
