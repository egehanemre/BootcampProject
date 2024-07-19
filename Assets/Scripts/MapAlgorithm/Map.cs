using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public Node[,] nodeArray;
    public Node finalNode;
    public List<Connection> connections = new List<Connection>();


    public void CreateConnection(Node node1, Node node2)
    {
        connections.Add(new Connection(node1, node2));
    }


    //NOT TESTED MAYBE CHECK FOR THE LEFT SIDE TOO
    public void RemoveConnection(Node node1, Node node2)
    {
        Connection connectionToRemove = null;

        foreach (Connection connection in connections)
        {
            if (connection.node1 == node1 && connection.node2 == node2)
            {
                connectionToRemove = connection;
            }
        }

        if (connectionToRemove != null)
        {
            connections.Remove(connectionToRemove);
        }
    }
}