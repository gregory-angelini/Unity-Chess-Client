using PopUp;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChessCore;

public class FigurePromotionPopUp : BasicPopUp
{
    [SerializeField] Sprite N;
    [SerializeField] Sprite B;
    [SerializeField] Sprite R;
    [SerializeField] Sprite Q;
   
    [SerializeField] Sprite n;
    [SerializeField] Sprite b;
    [SerializeField] Sprite r;
    [SerializeField] Sprite q;

    [SerializeField] Button knight;
    [SerializeField] Button bishop;
    [SerializeField] Button rook;
    [SerializeField] Button queen;


    public void Run(ChessCore.Color playerColor, Action<Figure> onPickFigure)
    {
        knight.onClick.AddListener(() => 
        { 
            onPickFigure?.Invoke(playerColor == ChessCore.Color.white ? Figure.whiteKnight : Figure.blackKnight); 
            CloseWindow(); 
        });
        bishop.onClick.AddListener(() => 
        {
            onPickFigure?.Invoke(playerColor == ChessCore.Color.white ? Figure.whiteBishop : Figure.blackBishop);
            CloseWindow(); 
        });
        rook.onClick.AddListener(() => 
        {
            onPickFigure?.Invoke(playerColor == ChessCore.Color.white ? Figure.whiteRook : Figure.blackRook);
            CloseWindow(); 
        });
        queen.onClick.AddListener(() => 
        {
            onPickFigure?.Invoke(playerColor == ChessCore.Color.white ? Figure.whiteQueen : Figure.blackQueen);
            CloseWindow(); 
        });

        if (playerColor == ChessCore.Color.white)
        {
            knight.image.sprite = N;
            bishop.image.sprite = B;
            rook.image.sprite = R;
            queen.image.sprite = Q;
        }
        else //if(playerColor == ChessCore.Color.black)
        {
            knight.image.sprite = n;
            bishop.image.sprite = b;
            rook.image.sprite = r;
            queen.image.sprite = q;
        }
    }
}
