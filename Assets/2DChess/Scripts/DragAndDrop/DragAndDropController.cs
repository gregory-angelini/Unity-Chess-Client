using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using ChessCore;
using System;
using ChessCore;

public class DragAndDropController : MonoBehaviour
{
    public static DragAndDropController Instance;

    public enum State
    {
        none,
        drag
    }
    public class DragArgs : EventArgs
    {
        public DraggableFigure draggedObject;
        public string fenMove;// move in FEN notation
        public Vector2 startDragPos = new Vector2();
        public Vector2 endDragPos = new Vector2();
        public bool result = false;
    }

    State state = State.none;
    DraggableFigure draggedObject;

    public event EventHandler<DragArgs> OnStartDragFigure;
    public event EventHandler<DragArgs> OnEndDragFigure;
    Vector2 startDragPos = new Vector2();
    Vector2 endDragPos = new Vector2();


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
            if (ClientController.Instance.PlayerColor != ClientController.Instance.GameState.lastMoveColor)
            {
                if (Chess2DController.Instance.Chess.GetCurrentPlayerColor() != Chess.GetFigureColor(obj.GetComponent<Figure2D>().Id))
                {
                    Debug.Log($"{Chess2DController.Instance.Chess.GetCurrentPlayerColor()} player must move next");
                    return;
                }

                draggedObject = obj;
                state = State.drag;

                Vector2 pos = (draggedObject.GetComponent<Figure2D>().GetWorldPosition() - Board2DBuilder.Instance.BoardStartPos) / Board2DBuilder.Instance.SquareSize;
                startDragPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));

                //Debug.Log($"Start drag: { draggedObject.name }: [{ StartDragPos.x }, { StartDragPos.y }]");

                DragArgs args = new DragArgs();
                args.draggedObject = draggedObject;
                args.startDragPos = startDragPos;
                args.result = true;
                OnStartDragFigure?.Invoke(this, args);
            }
        }
    }

    public void EndDrag()
    {
        if (IsDragging())
        {
            Vector2 pos = (draggedObject.GetComponent<Figure2D>().GetWorldPosition() - Board2DBuilder.Instance.BoardStartPos) / Board2DBuilder.Instance.SquareSize;
            endDragPos = new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));

            DragArgs args = new DragArgs();
            args.draggedObject = draggedObject;
            args.startDragPos = startDragPos;
            args.endDragPos = endDragPos;

            if (Chess2DController.Instance.Chess.CanMove(draggedObject.GetComponent<Figure2D>().Id,
                                                        (int)startDragPos.x, (int)startDragPos.y,
                                                        (int)endDragPos.x, (int)endDragPos.y))
            {
                string from = Chess.SquarePosToSquareName((int)startDragPos.x, (int)startDragPos.y);
                string to = Chess.SquarePosToSquareName((int)endDragPos.x, (int)endDragPos.y);
                Figure figure = Chess2DController.Instance.Chess.FigureAt((int)startDragPos.x, (int)startDragPos.y);
                string fenMove = (char)figure + from + to;
                args.fenMove = fenMove;
                args.result = true;
            }
            else
            {
                Vector2 oldPos = Board2DBuilder.Instance.GetWorldPosOfSquare((int)startDragPos.x, (int)startDragPos.y);
                draggedObject.GetComponent<Figure2D>().SetWorldPosition(oldPos);
            }
            
            OnEndDragFigure?.Invoke(this, args);

            draggedObject = null;
            state = State.none;
        }
    }
}
