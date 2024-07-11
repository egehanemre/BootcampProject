using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 position;
    public bool isVisited;

    public void ResetNode()
    {
        isVisited = false;
    }
}
