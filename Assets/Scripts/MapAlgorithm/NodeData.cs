using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NodeData", menuName = "NodeData", order = 1)]
public class NodeData : ScriptableObject
{
    public Sprite sprite;
    public GameObject[] enemyPrefabs; // Array of enemy prefabs for this node
    public bool isShop = false;
}