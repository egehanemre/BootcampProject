using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node
{
    public bool canMoveTo = false;

    public bool alreadyMoved = false;

    public Sprite sprite;

    public Vector2Int arrayPos;

    public bool isVisited = false;

    public GameObject[] enemyPrefabs;

    public NodeData nodeData;

    public GameObject associatedGameObject;
    public Node(Vector2Int arrayPosition)
    {
        arrayPos = arrayPosition;
    }

    public void SetNodeData(NodeData nodeData)
    {
        this.nodeData = nodeData;
        this.sprite = nodeData.sprite;
        this.enemyPrefabs = nodeData.enemyPrefabs;
        Debug.Log("NodeData: " + nodeData.sprite.name);
        Debug.Log("NodeData: " + nodeData.enemyPrefabs[0].name);
    }

    public Vector3 Position()
    {
        return new Vector3(arrayPos.x, arrayPos.y, 0);
    }
}