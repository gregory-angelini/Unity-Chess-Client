using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class BoardBuilder : MonoBehaviour
{
    float width = 1.5f;
    float length = 1.5f;
    Vector3 startPos = new Vector3(-5.25f, 0f, -5.25f);
    public Vector3[,] SquarePositions { get; private set; } = new Vector3[8, 8];

    void Start()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int z = 0; z < 8; z++)
            {
                SquarePositions[x, z] = startPos + new Vector3(width * x, 0, length * z);
            }
        }
    }

  
}
