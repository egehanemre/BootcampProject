using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeClick : MonoBehaviour, IPointerDownHandler
{
    public Node heldNode;
    private GameObject manager;
    public GameManager gameManager;
    private MapMovement mapMovement;

    public NodeData nodeData; // Assign this in the inspector
    private void Awake()
    {
        manager = GameObject.Find("Manager");
        mapMovement = manager.GetComponent<MapMovement>();
        gameManager = manager.GetComponent<GameManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (heldNode == null)
        {
            Debug.LogError("BIRADER BUNDA NASIL NODE YOK!"); ;
        }


        Debug.Log("Node Clicked: " + heldNode.arrayPos);


        mapMovement.nodeCheckDelegate(heldNode);
    }
}