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

    public Figure2D CreateFigure(char figureId)
    {
        Figure2D figureObj = Instantiate(figurePrefab);
        figureObj.SetFigure(figureId);
        return figureObj; 
    }

    public Sprite GetFigureSprite(char figureId)
    {
        switch (figureId)
        {
            case (char)Figure.whiteKing:
                return wKingSprite;

            case (char)Figure.blackKing:
                return bKingSprite;

            case (char)Figure.whiteQueen:
                return wQueenSprite;

            case (char)Figure.blackQueen:
                return bQueenSprite;

            case (char)Figure.whiteRook:
                return wRookSprite;

            case (char)Figure.blackRook:
                return bRookSprite;

            case (char)Figure.whiteBishop:
                return wBishopSprite;

            case (char)Figure.blackBishop:
                return bBishopSprite;

            case (char)Figure.whiteKnight:
                return wKnightSprite;

            case (char)Figure.blackKnight:
                return bKnightSprite;

            case (char)Figure.whitePawn:
                return wPawnSprite;

            case (char)Figure.blackPawn:
                return bPawnSprite;
        }
        return null;
    }
}
