using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessCore;


public class Chess2DController : MonoBehaviour
{
    public Chess Chess { get; private set; } = new Chess();
    public static Chess2DController Instance;
   
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        DragAndDropController.Instance.OnStartDrag += OnStartDragFigure;
        DragAndDropController.Instance.OnEndDrag += OnEndDragFigure;
    }

    void OnDestroy()
    {
        DragAndDropController.Instance.OnStartDrag -= OnStartDragFigure;
        DragAndDropController.Instance.OnEndDrag -= OnEndDragFigure;
    }

    void OnStartDragFigure(object source, DragAndDropController.DragArgs args)
    { }

    void OnEndDragFigure(object source, DragAndDropController.DragArgs args)
    {
        if (args.result)
        {
            Debug.Log($"End drag ({args.result }): { args.fenMove }");
            Chess = Chess.Move(args.fenMove);
            Board2DBuilder.Instance.UpdateBoard();
        }
    }
}
