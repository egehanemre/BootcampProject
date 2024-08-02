using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour
{

    public GameManager gameManager;
    private Showcaser showcaser;


    public Map map;
    public Node currentNode;

    public delegate void NodeCheckDelegate(Node node);
    public NodeCheckDelegate nodeCheckDelegate;


    private void Awake()
    {
        showcaser = GetComponent<Showcaser>();
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // Example assignment to demonstrate automatic delegation
        nodeCheckDelegate = CheckIfNodeIsAvailableToMove;
    }

    private void CheckIfNodeIsAvailableToMove(Node node)
    {
        if (node.canMoveTo)
        {
            MoveToNode(node);
        }
        else
        {
            Debug.Log("Node is not available to move");
        }

    }

    public void MoveToNode(Node node)
    {

        GameManager.isPlayerDoneSelectingThePointToMove = true;

        if (currentNode != null)
        {
            Debug.Log("Current node is not null");
            currentNode.alreadyMoved = true;
            currentNode.canMoveTo = true;
        }


        currentNode = node;
        gameManager.SetCurrentNodeData(node.nodeData);
        Debug.Log(currentNode.arrayPos);
        showcaser.HighlightCurrentAndAbleNodes();
    }
}