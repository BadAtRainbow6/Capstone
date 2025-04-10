using UnityEngine;

public class Node
{
    public Vector2Int coords;
    public bool walkable = true;
    public bool explored;
    public bool path;
    public Node connectedTo;

    public Node(Vector2Int coords, bool walkable)
    {
        this.coords = coords;
        this.walkable = walkable;
    }
}
