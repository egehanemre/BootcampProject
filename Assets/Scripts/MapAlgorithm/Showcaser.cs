using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(MapMovement))]
public class Showcaser : MonoBehaviour
{
    private Dictionary<GameObject, Coroutine> blinkingCoroutines = new Dictionary<GameObject, Coroutine>();

    public Map map;

    public GameObject NodePrefab;

    public GameObject parentCanvas;

    private MapMovement mapMovement;

    public bool isVisible = true;


    private bool firstMove = true;

    // private List<GameObject> InstantiatedNodes = new List<GameObject>();
    private List<GameObject> InstantiatedConnections = new List<GameObject>();

    private Dictionary<Node, GameObject> InstantiatedNodes = new Dictionary<Node, GameObject>();

    private void Awake()
    {
        mapMovement = GetComponent<MapMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleVisibilityAndClickability(!isVisible);
            isVisible = !isVisible;
        }
    }

    //eggy
    public void ToggleMapForGameScene()
    {
        ToggleVisibilityAndClickability(!isVisible);
        isVisible = !isVisible;
    }

    public void ShowMap()
    {
        isVisible = true;


        KillNodes();
        KillConnections();
        KillNodes();

        InstantiateNodes();
        InstantiateConnections();

        KillUnusedNodes();

        firstMove = true;
        HighlightCurrentAndAbleNodes();
    }

    public void InstantiateLastRoom()
    {
        Vector2 position = CalculateUIPosition(new Vector2(map.nodeArray.GetLength(0), map.nodeArray.GetLength(1) / 2) + new Vector2(0, 0));

        GameObject instantiatedNode = Instantiate(NodePrefab, position, Quaternion.identity, parentCanvas.transform);

        RectTransform rectTransform = instantiatedNode.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(100, 100);

        instantiatedNode.GetComponent<NodeClick>().heldNode = map.nodeArray[map.nodeArray.GetLength(0) - 1, map.nodeArray.GetLength(1) - 1];
        InstantiatedNodes.Add(map.finalNode, instantiatedNode);
        Debug.Log(map.finalNode.arrayPos);


    }

    public void ToggleVisibilityAndClickability(bool isVisible)
    {
        float alpha = isVisible ? 1.0f : 0.0f; // Set alpha to 1 for visible, 0 for invisible

        // Toggle clickability for nodes
        foreach (var nodePair in InstantiatedNodes)
        {
            // Check if the node exists before attempting to toggle its clickability
            if (nodePair.Value != null)
            {
                var nodeClick = nodePair.Value.GetComponent<NodeClick>();
                var canvasRenderer = nodePair.Value.GetComponent<CanvasRenderer>();
                if (nodeClick != null)
                {
                    nodeClick.enabled = isVisible;
                    canvasRenderer.SetAlpha(alpha);
                }
            }
        }

        // Assuming InstantiatedConnections is a List<GameObject> as shown in the context
        foreach (var connectionObject in InstantiatedConnections)
        {
            if (connectionObject != null) // Check if the connection object is not null
            {
                var lineRenderer = connectionObject.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    // Toggle visibility by enabling or disabling the LineRenderer component
                    lineRenderer.enabled = isVisible;
                }
            }
        }
    }





    private void InstantiateNodes()
    {
        foreach (Node node in map.nodeArray)
        {
            Vector2 position = CalculateUIPosition(node.arrayPos);

            GameObject instantiatedNode = Instantiate(NodePrefab, position, Quaternion.identity, parentCanvas.transform);

            RectTransform rectTransform = instantiatedNode.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(100, 100);

            if (node.sprite != null)
            {


                instantiatedNode.GetComponent<Image>().sprite = node.sprite;
            }
            else
            {
                Debug.Log("NODE IMAGE IS NULL");
            }


            instantiatedNode.GetComponent<NodeClick>().heldNode = node;
            // Add the instantiated node to the dictionary
            InstantiatedNodes.Add(node, instantiatedNode);
        }

    }

    private Vector2 CalculateUIPosition(Vector2 arrayPos)
    {
        return new Vector2(arrayPos.x * 100, arrayPos.y * 100);
    }




    private void KillNodes()
    {
        foreach (GameObject nodeObject in InstantiatedNodes.Values)
        {
            Destroy(nodeObject);
        }
        InstantiatedNodes.Clear(); // Clear the dictionary after destroying the objects
    }

    private void KillUnusedNodes()
    {
        foreach (KeyValuePair<Node, GameObject> node in InstantiatedNodes)
        {
            if (!node.Key.isVisited)
            {
                Destroy(node.Value);
            }
        }
    }




    private void InstantiateConnections()
    {
        foreach (Connection connection in map.connections)
        {
            // Convert node positions from world space to canvas space
            Vector2 node1PositionOnCanvas = CalculateUIPosition(connection.node1.arrayPos);
            Vector2 node2PositionOnCanvas = CalculateUIPosition(connection.node2.arrayPos);

            // Create a GameObject to hold the LineRenderer
            GameObject connectionObject = new GameObject("Connection");
            connectionObject.transform.SetParent(parentCanvas.transform, false);

            LineRenderer lineRenderer = connectionObject.AddComponent<LineRenderer>();

            // Set material and color as needed, for UI compatibility
            Material uiMaterial = new Material(Shader.Find("UI/Default")); // Adjust shader for UI compatibility
            lineRenderer.material = uiMaterial;
            lineRenderer.startColor = lineRenderer.endColor = Color.white;

            lineRenderer.positionCount = 2;

            // Adjust Z-position to ensure visibility in UI
            float zPosition = -1f; // Adjust based on your UI camera setup
            Vector3 startPosition = parentCanvas.transform.TransformPoint(new Vector3(node1PositionOnCanvas.x, node1PositionOnCanvas.y, zPosition));
            Vector3 endPosition = parentCanvas.transform.TransformPoint(new Vector3(node2PositionOnCanvas.x, node2PositionOnCanvas.y, zPosition));

            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);

            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;

            lineRenderer.sortingOrder = -1;
            // Adjust based on your canvas type
            lineRenderer.useWorldSpace = true; // Change to false if using Screen Space - Camera or World Space canvas

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

    public void HighlightCurrentAndAbleNodes()
    {
        if (!firstMove)
        {
            foreach (GameObject node in InstantiatedNodes.Values)
            {
                if (node != null)
                {
                    node.GetComponent<NodeClick>().heldNode.canMoveTo = false;
                }
            }
        }
        firstMove = false;

        if (mapMovement.currentNode != null)
        {
            foreach (Connection connection in map.connections)
            {
                Node targetNode = null;
                if (connection.node1 == mapMovement.currentNode && connection.node2.arrayPos.x > mapMovement.currentNode.arrayPos.x)
                {
                    targetNode = connection.node2;
                }
                else if (connection.node2 == mapMovement.currentNode && connection.node1.arrayPos.x > mapMovement.currentNode.arrayPos.x)
                {
                    targetNode = connection.node1;
                }

                if (targetNode != null && InstantiatedNodes.ContainsKey(targetNode))
                {
                    GameObject nodeObj = InstantiatedNodes[targetNode];
                    nodeObj.GetComponent<NodeClick>().heldNode.canMoveTo = true;
                }
            }
        }

        foreach (GameObject node in InstantiatedNodes.Values)
        {
            if (node != null)
            {
                NodeClick nodeClick = node.GetComponent<NodeClick>();
                if (nodeClick.heldNode.canMoveTo)
                {
                    node.GetComponent<Image>().color = Color.green;
                    if (!blinkingCoroutines.ContainsKey(node))
                    {
                        Coroutine blinkCoroutine = StartCoroutine(BlinkNode(node));
                        blinkingCoroutines[node] = blinkCoroutine;
                    }
                }
                else
                {
                    node.GetComponent<Image>().color = Color.white;
                    if (blinkingCoroutines.ContainsKey(node))
                    {
                        StopCoroutine(blinkingCoroutines[node]);
                        blinkingCoroutines.Remove(node);
                    }
                    node.transform.localScale = Vector3.one; // Reset scale
                }
                if (nodeClick.heldNode.alreadyMoved)
                {
                    node.GetComponent<Image>().color = Color.gray;
                }
                if (nodeClick.heldNode == mapMovement.currentNode)
                {
                    node.GetComponent<Image>().color = Color.blue;
                }
            }
        }
    }

    private IEnumerator BlinkNode(GameObject node)
    {
        while (true)
        {
            // Scale up
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                node.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, t);
                yield return null;
            }
            // Scale down
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                node.transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.one, t);
                yield return null;
            }
        }
    }
}
