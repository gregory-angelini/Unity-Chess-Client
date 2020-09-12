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
    [SerializeField] Transform figureParent;
    [SerializeField] Transform verticalLabels;
    [SerializeField] Transform horizontalLabels;
    Quaternion figureRot = Quaternion.identity;
    [SerializeField] Transform HUDParent;
    [SerializeField] HighlightSquare squarePrefab;
    HighlightSquare[,] squares = new HighlightSquare[8, 8];


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void HideMoves()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                squares[x, y].Hide();
            }
        }
    }

    public void ShowLegalSquare(int x, int y, bool emptyOrEnemy)
    {
        if (emptyOrEnemy)
            squares[x, y].ShowEmptySquare();
        else
            squares[x, y].ShowEnemySquare();
    }

    void CreateSquares()
    {
        HighlightSquare square;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                square = Instantiate(squarePrefab);
                square.transform.SetParent(HUDParent, false);

                square.Hide();

                Vector3 pos = Board2DBuilder.Instance.BoardStartPos + new Vector2(x * Board2DBuilder.Instance.SquareSize.x, y * Board2DBuilder.Instance.SquareSize.y);
                square.SetWorldPosition(pos);
                squares[x, y] = square;
            }
        }
    }

    public Vector2 GetWorldPosOfSquare(int x, int y)
    {
        return BoardStartPos + new Vector2(x * SquareSize.x, y * SquareSize.y);
    }


    public void FlipBoard()
    {
        HUDParent.transform.localRotation *= Quaternion.Euler(0, 0, 180);
        figureParent.localRotation *= Quaternion.Euler(0, 0, 180);
        figureRot *= Quaternion.Euler(0, 0, 180);
        Debug.Log("q:" + figureRot);

        foreach (var row in figures.Rows())
            foreach (var figure in row)
                if (figure != null)
                {
                    figure.SetRotation(figureRot);
                }


        verticalLabels.localRotation *= Quaternion.Euler(180, 0, 0);
        foreach(Transform label in verticalLabels)
        {
            label.localRotation *= Quaternion.Euler(180, 0, 0);
        }

        horizontalLabels.localRotation *= Quaternion.Euler(0, 180, 0);

        foreach (Transform label in horizontalLabels)
        {
            label.localRotation *= Quaternion.Euler(0, 180, 0);
        }
    }

    public void UpdateBoard()
    {
        Vector2 pos;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Figure figureId = Chess2DController.Instance.Chess.FigureAt(x, y);

                if (figureId == Figure.none)
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
                        if (figureId == figures[x, y].Id)// no changes
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


    public void Build(Chess chess)
    {
        CreateSquares();

        //Debug.Log("New game");

        Vector2 pos;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // clear the square 
                if (figures[x, y] != null)
                {
                    Destroy(figures[x, y].gameObject);
                    figures[x, y] = null;
                }

                Figure figureId = chess.FigureAt(x, y);

                if (figureId != Figure.none)
                {
                    pos = BoardStartPos + new Vector2(x * SquareSize.x, y * SquareSize.y);
                    Figure2D figureObj = CreateFigure(figureId, pos, x, y);
                    figures[x, y] = figureObj;
                }
            }
        }
    }

    Figure2D CreateFigure(Figure figureId, Vector2 pos, int x, int y)
    {
        Figure2D figureObj = Figure2DBuilder.Instance.CreateFigure(figureId);
        figureObj.transform.SetParent(figureParent, false);
        

        if (figureObj == null) throw new System.IndexOutOfRangeException($"Cannot create a figure '{figureId}' in position [{x}, {y}]");
        
        figureObj.SetPosition(pos);
        figureObj.SetRotation(figureRot);
        figureObj.XBoardPos = x;
        figureObj.YBoardPos = y;
        return figureObj;
    }
}
