using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using ChessCore;


public class Figure2DBuilder : MonoBehaviour
{
    [SerializeField] Figure2D figurePrefab;

    [SerializeField] Sprite wPawnSprite;
    [SerializeField] Sprite wKnightSprite;
    [SerializeField] Sprite wBishopSprite;
    [SerializeField] Sprite wRookSprite;
    [SerializeField] Sprite wKingSprite;
    [SerializeField] Sprite wQueenSprite;

    [SerializeField] Sprite bPawnSprite;
    [SerializeField] Sprite bKnightSprite;
    [SerializeField] Sprite bBishopSprite;
    [SerializeField] Sprite bRookSprite;
    [SerializeField] Sprite bKingSprite;
    [SerializeField] Sprite bQueenSprite;

    public static Figure2DBuilder Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Figure2D CreateFigure(Figure figureId)
    {
        Figure2D figureObj = Instantiate(figurePrefab);
        figureObj.SetFigure(figureId);
        return figureObj; 
    }

    public Sprite GetFigureSprite(Figure figureId)
    {
        switch (figureId)
        {
            case Figure.whiteKing:
                return wKingSprite;

            case Figure.blackKing:
                return bKingSprite;

            case Figure.whiteQueen:
                return wQueenSprite;

            case Figure.blackQueen:
                return bQueenSprite;

            case Figure.whiteRook:
                return wRookSprite;

            case Figure.blackRook:
                return bRookSprite;

            case Figure.whiteBishop:
                return wBishopSprite;

            case Figure.blackBishop:
                return bBishopSprite;

            case Figure.whiteKnight:
                return wKnightSprite;

            case Figure.blackKnight:
                return bKnightSprite;

            case Figure.whitePawn:
                return wPawnSprite;

            case Figure.blackPawn:
                return bPawnSprite;
        }
        return null;
    }
}
