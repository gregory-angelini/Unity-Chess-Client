using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessCore;

public class Board2DBuilder : MonoBehaviour
{
    Chess chess = new Chess();
    Vector2 startPos = new Vector3(-415, -415);
    List<Figure2D> figures = new List<Figure2D>();
    float squareWidth = 118f;
    float squareHeight = 118f;
    public static Board2DBuilder Instance;
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

    public void Build()
    {
        // clear the board
        while(figures.Count > 0)
        {
            Destroy(figures[0].gameObject);
            figures.RemoveAt(0);
        }

        Vector2 pos;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                char figureCode = chess.FigureAt(x, y);
                if (figureCode == '.') continue;

                pos = startPos + new Vector2(x * squareWidth, y * squareHeight);

                Figure2D figureObj = CreateFigure(figureCode, pos, x, y);
                figures.Add(figureObj);
            }
        }
    }

    Figure2D CreateFigure(char figureCode, Vector2 pos, int x, int y)
    {
        Figure2D figureObj = Figure2DBuilder.Instance.CreateFigure(figureCode);
        figureObj.transform.parent = parent;
        
        if (figureObj == null) throw new System.IndexOutOfRangeException($"Cannot create a figure '{figureCode}' in position [{x}, {y}]");
        
        figureObj.SetWorldPosition(pos);
        figureObj.XBoardPos = x;
        figureObj.YBoardPos = y;
        return figureObj;
    }
}
