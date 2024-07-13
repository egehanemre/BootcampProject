using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Showcaser : MonoBehaviour
{
    public Map map;

    public GameObject NodePrefab;

    // private List<GameObject> InstantiatedNodes = new List<GameObject>();
    private List<GameObject> InstantiatedConnections = new List<GameObject>();

    private Dictionary<Node, GameObject> InstantiatedNodes = new Dictionary<Node, GameObject>();

    private void Awake()
    {

    }

    public void ShowMap()
    {
        KillNodes();
        KillConnections();

        InstantiateNodes();
        InstantiateConnections();

        KillUnusedNodes();
    }

    private void KillUnusedNodes()
    {
        foreach (Node node in map.nodeArray)
        {
            if (node.isVisited == false)
            {
                GameObject.Destroy(GameObject.Find("Node " + node.arrayPos.x + ", " + node.arrayPos.y));
            }
        }
    }

    private void InstantiateNodes()
    {
        foreach (Node node in map.nodeArray)
        {
            Vector3 instPos = new Vector3(node.arrayPos.x, node.arrayPos.y, 0);
            GameObject nodeObject = Instantiate(NodePrefab, instPos, Quaternion.identity);
            nodeObject.transform.SetParent(transform);
            nodeObject.name = "Node " + node.arrayPos.x + ", " + node.arrayPos.y;
            // Add the node and its GameObject to the dictionary
            InstantiatedNodes.Add(node, nodeObject);
        }

        VisualizeLastRoom();
    }

    private void VisualizeLastRoom()
    {
        Vector3 instPos = new Vector3(map.nodeArray.GetLength(0), (map.nodeArray.GetLength(1)) / 2, 0);
        GameObject nodeObject = Instantiate(NodePrefab, instPos, Quaternion.identity);
        nodeObject.transform.SetParent(transform);
        nodeObject.name = "BOSS NODE";

        InstantiatedNodes.Add(new Node(new Vector2Int(map.nodeArray.Length - 1, (map.nodeArray.Length - 1) / 2)), nodeObject);

    }


    private void KillNodes()
    {
        // Create a list to hold nodes that need to be removed from the dictionary
        List<Node> nodesToRemove = new List<Node>();

        foreach (var entry in InstantiatedNodes)
        {
            if (!entry.Key.isVisited) // Check if the node has not been visited
            {
                Destroy(entry.Value);
                nodesToRemove.Add(entry.Key);
            }
        }

        // Remove the nodes from the dictionary
        foreach (Node node in nodesToRemove)
        {
            InstantiatedNodes.Remove(node);
        }
    }




    private void InstantiateConnections()
    {
        foreach (Connection connection in map.connections)
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = connection.node1.Position();
            positions[1] = connection.node2.Position();

            GameObject connectionObject = new GameObject("Connection");
            connectionObject.transform.SetParent(transform);

            LineRenderer lineRenderer = connectionObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(positions);
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.useWorldSpace = false; // Add this line to use local space for positions

            InstantiatedConnections.Add(connectionObject);
        }
    }


    private void KillConnections()
    {
        foreach (GameObject connectionObject in InstantiatedConnections)
        {
            Destroy(connectionObject);
        }
        InstantiatedConnections.Clear(); // Clear the list after destroying the objects
    }
}