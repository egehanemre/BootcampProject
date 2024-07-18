using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDataInserter : MonoBehaviour
{
    public List<NodeData> nodeDatas = new List<NodeData>();

    public Map currentMap;


    //GENERATOR SCRIPT TE ÇALIŞTIRIŞIYOR (GENERATE MAP FONKSIYONU)
    public void InsertRandomDataToAllNodes()
    {
        currentMap = GameObject.Find("Manager").GetComponent<Generator>().currentMap;

        foreach (Node node in currentMap.nodeArray)
        {
            NodeData data = nodeDatas[Random.Range(0, nodeDatas.Count)];
            node.SetNodeData(data);
        }
    }
}