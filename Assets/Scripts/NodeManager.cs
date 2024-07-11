using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NodeManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int rows = 5;
    public int columns = 10;
    public float spacingX = 2.0f;
    public float spacingY = 2.0f;
    public int startX = -8;
    public int startY = -4;

    private Node[,] nodes;
    private List<List<Node>> nodesInColumns = new List<List<Node>>();
    private List<List<Node>> nodesInRows = new List<List<Node>>();

    void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        ClearGrid();

        nodes = new Node[rows, columns];
        nodesInColumns.Clear();
        nodesInRows.Clear();

        for (int x = 0; x < columns; x++)
        {
            List<Node> columnNodes = new List<Node>();
            for (int y = 0; y < rows; y++)
            {
                if (x == 0)
                {
                    nodesInRows.Add(new List<Node>());
                }


                Vector2 position = new Vector2(startX + x * spacingX, startY + y * spacingY);
                GameObject nodeObject = Instantiate(nodePrefab, position, Quaternion.identity);
                nodeObject.transform.parent = transform; // Keep the hierarchy clean
                Node node = nodeObject.GetComponent<Node>();
                node.position = position;
                nodes[y, x] = node;
                columnNodes.Add(node);
                nodesInRows[y].Add(node);
            }
            nodesInColumns.Add(columnNodes);
        }
    }

    private void ClearGrid()
    {
        if (nodes != null)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (nodes[y, x] != null)
                    {
                        Destroy(nodes[y, x].gameObject);
                    }
                }
            }
        }
        nodes = null;
        nodesInColumns.Clear();
        nodesInRows.Clear();
    }

    public void WeldIteration()
{
    int randomColumn = Random.Range(0, nodesInColumns.Count);
    int randomRow = Random.Range(0, nodesInColumns[randomColumn].Count - 1);

    Node node1 = nodesInColumns[randomColumn][randomRow];
    Node node2 = nodesInColumns[randomColumn][randomRow + 1];

    // Remove node2 from the column
    nodesInColumns[randomColumn].RemoveAt(randomRow + 1);

    // Average the positions
    Vector2 averagedPosition = 0.5f * (node1.position + node2.position);
    node1.position = averagedPosition;
    node2.position = averagedPosition;

    // Update the position of the GameObjects
    node1.gameObject.transform.position = averagedPosition;
    node2.gameObject.transform.position = averagedPosition;
}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (nodes != null)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (nodes[y, x] != null)
                    {
                        Gizmos.DrawSphere(nodes[y, x].position, 0.1f);
                        if (x < columns - 1 && nodes[y, x + 1] != null)
                        {
                            Gizmos.DrawLine(nodes[y, x].position, nodes[y, x + 1].position);
                        }
                    }
                }
            }
        }
    }
}