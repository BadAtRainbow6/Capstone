using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    [SerializeField] Vector2Int startCoords;
    public Vector2Int StartCoords { get { return startCoords; } }

    [SerializeField] Vector2Int targetCoords;
    public Vector2Int TargetCoords { get { return targetCoords; } }

    Node startNode;
    Node targetNode;
    Node currentNode;


    Queue<Node> frontier = new Queue<Node>();
    Dictionary<Vector2Int, Node> reached = new Dictionary<Vector2Int, Node>();

    GridManager gridManager = new GridManager();
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    Vector2Int[] searchOrder = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        if(gridManager != null)
        {
            grid = gridManager.Grid;
        }
    }

    public List<Node> GetNewPath()
    {
        return GetNewPath(startCoords);
    }

    public List<Node> GetNewPath(Vector2Int coords)
    {
        gridManager.ResetNodes();

        BreadthFirstSearch(coords);
        return BuildPath();
    }

    void BreadthFirstSearch(Vector2Int coords)
    {
        startNode.walkable = true;

        frontier.Clear();
        reached.Clear();

        bool isRunning = true;

        frontier.Enqueue(grid[coords]);
        reached.Add(coords, grid[coords]);

        while(frontier.Count > 0 && isRunning)
        {
            currentNode = frontier.Dequeue();
            currentNode.explored = true;
            ExploreNeighbors();
            if(currentNode.coords == targetCoords)
            {
                isRunning = false;
            }
        }
    }
    
    void ExploreNeighbors()
    {
        List<Node> neighbors = new List<Node>();

        foreach(Vector2Int direction in searchOrder)
        {
            Vector2Int neighborCoords = currentNode.coords + direction;

            if(grid.ContainsKey(neighborCoords))
            {
                neighbors.Add(grid[neighborCoords]);
            }
        }

        foreach (Node neighbor in neighbors)
        {
            if(!reached.ContainsKey(neighbor.coords) && neighbor.walkable == true)
            {
                neighbor.connectedTo = currentNode;
                reached.Add(neighbor.coords, neighbor);
                frontier.Enqueue(neighbor);
            }
        }
    }

    List<Node> BuildPath()
    {
        List<Node> path = new List<Node>();
        Node pathCurrentNode = targetNode;

        path.Add(pathCurrentNode);
        pathCurrentNode.path = true;

        while(pathCurrentNode.connectedTo != null)
        {
            pathCurrentNode = pathCurrentNode.connectedTo;
            path.Add(pathCurrentNode);
            pathCurrentNode.path = true;
        }

        path.Reverse();
        return path;
    }

    public void NotifyRecievers()
    {
        BroadcastMessage("RecalculatePath", false, SendMessageOptions.DontRequireReceiver);
    }

    public void SetNewDestination(Vector2Int startCoords, Vector2Int targetCoords)
    {
        this.startCoords = startCoords;
        this.targetCoords = targetCoords;
        startNode = grid[this.startCoords];
        targetNode = grid[this.targetCoords];
    }
}
