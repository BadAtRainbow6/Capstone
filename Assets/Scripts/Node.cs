using UnityEngine;

public class Node
{
    public Vector2Int coords;
    public bool walkable;
    public bool explored;
    public bool path;
    public Node conectedTo;

    public Node(Vector2Int coords, bool walkable)
    {
        this.coords = coords;
        this.walkable = walkable;
    }
}
