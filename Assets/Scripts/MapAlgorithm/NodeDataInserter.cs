using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDataInserter : MonoBehaviour
{
    public List<NodeData> nodeDatas = new List<NodeData>();
    public List<NodeData> floor1NodeDatas = new List<NodeData>();
    public List<NodeData> floor2NodeDatas = new List<NodeData>();
    public List<NodeData> floor3NodeDatas = new List<NodeData>();
    public List<NodeData> floor4NodeDatas = new List<NodeData>();
    public List<NodeData> floor5NodeDatas = new List<NodeData>();
    public List<NodeData> floor6NodeDatas = new List<NodeData>();
    public List<NodeData> floor7NodeDatas = new List<NodeData>();
    public List<NodeData> floor8NodeDatas = new List<NodeData>();
    public List<NodeData> floor9NodeDatas = new List<NodeData>();
    public List<NodeData> floor10NodeDatas = new List<NodeData>();
    public List<NodeData> floor11NodeDatas = new List<NodeData>();
    public List<NodeData> floor12NodeDatas = new List<NodeData>();

    public Map currentMap;

    //GENERATOR SCRIPT TE ÇALIŞTIRIŞIYOR (GENERATE MAP FONKSIYONU)

    public void InsertDatas()
    {
        currentMap = GameObject.Find("Manager").GetComponent<Generator>().currentMap;
        InsertNodesToFloor(0, floor1NodeDatas);
        InsertNodesToFloor(1, floor2NodeDatas);
        InsertNodesToFloor(2, floor3NodeDatas);
        InsertNodesToFloor(3, floor4NodeDatas);
        InsertNodesToFloor(4, floor5NodeDatas);
        InsertNodesToFloor(5, floor6NodeDatas);
        InsertNodesToFloor(6, floor7NodeDatas);
        InsertNodesToFloor(7, floor8NodeDatas);
        InsertNodesToFloor(8, floor9NodeDatas);
        InsertNodesToFloor(9, floor10NodeDatas);
        InsertNodesToFloor(10, floor11NodeDatas);
        InsertNodesToFloor(11, floor12NodeDatas);
    }

    public void InsertNodesToFloor(int floorIndex, List<NodeData> list)
    {
        for (int i = 0; i < currentMap.nodeArray.GetLength(1); i++)
        {
            if(currentMap.nodeArray[floorIndex,i] != null)
            {
                NodeData nodeData = list[Random.Range(0, list.Count)];
                currentMap.nodeArray[floorIndex, i].SetNodeData(nodeData);
            }
        }
    }
}