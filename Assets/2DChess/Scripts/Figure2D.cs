using ChessCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Figure2D : MonoBehaviour
{
    Image image;
    public Figure Id { get; private set; } = Figure.none;
   
    int xBoardPos = -1;
    public int XBoardPos
    {
        get { return xBoardPos; }
        set
        {
            if (value >= 0 && value < 8)
                xBoardPos = value;
        }
    }

    int yBoardPos = -1;
    public int YBoardPos
    {
        get { return yBoardPos; }
        set
        {
            if (value >= 0 && value < 8)
                yBoardPos = value;
        }
    }

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetWorldPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    public Vector2 GetWorldPosition()
    {
        return transform.localPosition;
    }

    public void SetFigure(char figureId)
    {
        Sprite sprite = Figure2DBuilder.Instance.GetFigureSprite(figureId);

        if (sprite != null)
        {
            SetSprite(sprite);
            
            Id = (Figure)figureId;
        }
    }

    void SetSprite(Sprite image)
    {
        this.image.sprite = image;
    }
}
