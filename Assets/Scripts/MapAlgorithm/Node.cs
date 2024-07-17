using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int arrayPos;

    public bool isVisited = false;

    public Node(Vector2Int arrayPosition)
    {
        arrayPos = arrayPosition;
    }

    public Vector3 Position()
    {
        return new Vector3(arrayPos.x, arrayPos.y, 0);
    }
}