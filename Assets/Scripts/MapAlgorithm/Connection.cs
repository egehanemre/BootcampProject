using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
{
    public Node node1;
    public Node node2;

    public Connection(Node node1, Node node2)
    {
        this.node1 = node1;
        this.node2 = node2;
    }
}