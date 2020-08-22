using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessCore;

public class Board2DBuilder : MonoBehaviour
{
    public static Board2DBuilder Instance;
    public Vector2 BoardStartPos { get; private set; } = new Vector2(-415, -415);
    Figure2D[,] figures = new Figure2D[8, 8];
    public Vector2 SquareSize { get; private set; } = new Vector2(118f, 118f);
    [SerializeField] Transform parent;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Build();
    }

    public Vector2 GetWorldPosOfSquare(int x, int y)
    {
        return BoardStartPos + new Vector2(x * SquareSize.x, y * SquareSize.y);
    }

    public void UpdateBoard()
    {
        Vector2 pos;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                char figureId = Chess2DController.Instance.Chess.FigureAt(x, y);

                if (figureId == '.')
                {
                    if (figures[x, y] != null)// figure is gone
                    {
                        Destroy(figures[x, y].gameObject);
                        figures[x, y] = null;
                    }
                }
                else
                {
                    if (figures[x, y] != null)
                    {
                        if (figureId == (char)figures[x, y].Id)// no changes
                        {
                            continue;
                        }
                        else// figure has been changed
                        {
                            Destroy(figures[x, y].gameObject);
                            figures[x, y] = null;
                        }
                    }
                    // create new figure        
                    pos = BoardStartPos + new Vector2(x * SquareSize.x, y * SquareSize.y);
                    Figure2D figureObj = CreateFigure(figureId, pos, x, y);
                    figures[x, y] = figureObj;
                }
            }
        }
    }


    void Build()
    {
        Vector2 pos;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                char figureId = Chess2DController.Instance.Chess.FigureAt(x, y);

                if (figureId == '.')
                {
                    figures[x, y] = null;
                }
                else
                {
                    pos = BoardStartPos + new Vector2(x * SquareSize.x, y * SquareSize.y);
                    Figure2D figureObj = CreateFigure(figureId, pos, x, y);
                    figures[x, y] = figureObj;
                }
            }
        }
    }

    Figure2D CreateFigure(char figureId, Vector2 pos, int x, int y)
    {
        Figure2D figureObj = Figure2DBuilder.Instance.CreateFigure(figureId);
        figureObj.transform.SetParent(parent, false);
        
        if (figureObj == null) throw new System.IndexOutOfRangeException($"Cannot create a figure '{figureId}' in position [{x}, {y}]");
        
        figureObj.SetWorldPosition(pos);
        figureObj.XBoardPos = x;
        figureObj.YBoardPos = y;
        return figureObj;
    }
}
