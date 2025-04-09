using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;

    //[SerializeField];

    [SerializeField] bool premade;
    [SerializeField] GameObject[] tiles;
    public int UnityGridSize {  get { return unityGridSize; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Dictionary<Vector2Int, Node> Grid {  get { return grid; } }

    private void Awake()
    {
        CreateGrid();
    }

    public Node GetNode(Vector2Int coords)
    {
        if (grid.ContainsKey(coords)) return grid[coords];

        return null;
    }

    public void BlockNode(Vector2Int coords)
    {
        if (grid.ContainsKey(coords)) grid[coords].walkable = false;
    }

    public void ResetNodes()
    {
        foreach(KeyValuePair<Vector2Int, Node> coord in grid)
        {
            coord.Value.conectedTo = null;
            coord.Value.explored = false;
            coord.Value.path = false;
        }
    }

    public Vector2Int GetCoordsFromPos(Vector3 pos)
    {
        Vector2Int coords = new Vector2Int();


    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (!premade)
                {
                    Instantiate(tiles[Random.Range(0, tiles.Length - 1)], new Vector3(x * unityGridSize, 0, y * unityGridSize), Quaternion.identity);
                }

                Vector2Int coords = new Vector2Int(x, y);
                grid.Add(coords, new Node(coords, true));
            }
        }
    }
}
