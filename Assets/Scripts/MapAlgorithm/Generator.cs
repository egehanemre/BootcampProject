using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Showcaser))]
public class Generator : MonoBehaviour
{
    private Showcaser showcaser;

    public int GRID_WIDTH = 10;
    public int GRID_HEIGHT = 10;

    public int pathCount = 3;

    public Map currentMap;


    private void Awake()
    {
        showcaser = GetComponent<Showcaser>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateMap();

            showcaser.map = currentMap;
            showcaser.ShowMap();
        }
    }


    private void GenerateMap()
    {
        currentMap = new Map();

        GenerateGridOnMap();


        for (int i = 0; i < pathCount; i++)
        {
            GenerateConnectionsOnMap();
        }

        CreateLastRoomAndConnect();

    }


    private void GenerateGridOnMap()
    {
        currentMap.nodeArray = new Node[GRID_WIDTH, GRID_HEIGHT];

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                currentMap.nodeArray[x, y] = new Node(new Vector2Int(x, y));
            }
        }
    }

    private void GenerateConnectionsOnMap()
    {
        int pathCompleted = 0;
        Vector2Int currentArrayPos = new Vector2Int(0, UnityEngine.Random.Range(0, GRID_HEIGHT));
        int loopCount = 0;
        while (pathCompleted < (GRID_WIDTH / 2) && loopCount < 1000)
        {
            loopCount++;
            if (!AttemptToConnectAdjacentNodes(ref currentArrayPos))
            {
                break; // Break if no connections can be made to avoid infinite loop
            }
        }
        Debug.Log($"Path Completed: {pathCompleted} Loop Count: {loopCount}");
    }

    private bool AttemptToConnectAdjacentNodes(ref Vector2Int currentArrayPos)
    {
        List<Node> nodesToConnect = new List<Node>();
        if (currentArrayPos.x < GRID_WIDTH - 1)
        {
            AddPossibleConnections(ref nodesToConnect, currentArrayPos);
            if (nodesToConnect.Count > 0)
            {
                Node nodeToConnect = nodesToConnect[UnityEngine.Random.Range(0, nodesToConnect.Count)];
                currentMap.CreateConnection(currentMap.nodeArray[currentArrayPos.x, currentArrayPos.y], nodeToConnect);
                nodeToConnect.isVisited = true; // Mark the connected node as visited
                currentArrayPos = nodeToConnect.arrayPos;
                return true;
            }
        }
        return false;
    }

    private void AddPossibleConnections(ref List<Node> nodesToConnect, Vector2Int currentArrayPos)
    {
        nodesToConnect.Add(currentMap.nodeArray[currentArrayPos.x + 1, currentArrayPos.y]);
        if (currentArrayPos.y > 0)
        {
            nodesToConnect.Add(currentMap.nodeArray[currentArrayPos.x + 1, currentArrayPos.y - 1]);
        }
        if (currentArrayPos.y < GRID_HEIGHT - 1)
        {
            nodesToConnect.Add(currentMap.nodeArray[currentArrayPos.x + 1, currentArrayPos.y + 1]);
        }
        currentMap.nodeArray[currentArrayPos.x, currentArrayPos.y].isVisited = true; // This line already marks the current node as visited
    }


    private void CreateLastRoomAndConnect()
    {
        // The new room will be placed at GRID_WIDTH (to the right of the last column).
        Node lastRoom = new Node(new Vector2Int(GRID_WIDTH, GRID_HEIGHT / 2)); // Adjusted for consistency.

        for (int y = 0; y < GRID_HEIGHT; y++)
        {
            if (currentMap.nodeArray[GRID_WIDTH - 1, y].isVisited == true)
            {
                Node roomToConnect = currentMap.nodeArray[GRID_WIDTH - 1, y]; // This is now the last room in each row.
                ConnectRooms(roomToConnect, lastRoom);
            }


        }

        // Optionally, add the last room to a collection of rooms if you're keeping track of all rooms/nodes separately.
    }

    // Example method for connecting two rooms. You'll need to adjust this based on how your Node class and connections are structured.
    private void ConnectRooms(Node room1, Node room2)
    {
        currentMap.CreateConnection(room1, room2);

    }

}