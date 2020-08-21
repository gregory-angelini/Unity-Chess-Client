using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;

public class DragAndDropController : MonoBehaviour
{
    public static DragAndDropController Instance;

    public enum State
    {
        none,
        drag
    }

    State state = State.none;
    DraggableFigure draggedObject;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool IsDragging()
    {
        return state == State.drag;
    }
    public void StartDrag(DraggableFigure obj)
    {
        if (state == State.none)
        {
            Debug.Log($"Start drag: { name } ");
            this.draggedObject = obj;
            state = State.drag;
        }
    }

    public void EndDrag()
    {
        if (IsDragging())
        {
            Debug.Log($"End drag: { name } ");
            this.draggedObject = null;
            state = State.none;
        }
    }
}
