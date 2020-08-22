using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HighlightSquare : MonoBehaviour
{
    [SerializeField] Color emptyColor;
    [SerializeField] Color enemyColor;
    Color invisibleColor = new Color(1, 1, 1, 0);

    public void ShowEmptySquare()
    {
        GetComponent<Image>().color = emptyColor;
    }

    public void ShowEnemySquare()
    {
        GetComponent<Image>().color = enemyColor;
    }

    public void Hide()
    {
        GetComponent<Image>().color = invisibleColor;
    }

    public void SetWorldPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }
}
