using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FigurePromotionPopUp : MonoBehaviour
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

    Action OnPickKnight;
    Action OnPickBishop;
    Action OnPickRook;
    Action OnPickQueen;
    ChessCore.Color playerColor;

    public void Init(ChessCore.Color playerColor, Action onPickKnight, Action onPickBishop, Action onPickRook, Action onPickQueen)
    {
        this.playerColor = playerColor;
        OnPickKnight = onPickKnight;
        OnPickBishop = onPickBishop;
        OnPickRook = onPickRook;
        OnPickQueen = onPickQueen;

        knight.onClick.AddListener(OnKnightButton);
        bishop.onClick.AddListener(OnBishopButton);
        rook.onClick.AddListener(OnRookButton);
        queen.onClick.AddListener(OnQueenButton);

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
    
    void OnKnightButton()
    {
        OnPickKnight?.Invoke();
    }

    void OnBishopButton()
    {
        OnPickBishop?.Invoke();
    }

    void OnRookButton()
    {
        OnPickRook?.Invoke();
    }

    void OnQueenButton()
    {
        OnPickQueen?.Invoke();
    }
}
