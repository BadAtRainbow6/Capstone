using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : NetworkBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;

    [SerializeField] bool premade;
    [SerializeField] GameObject[] tiles;
    public int UnityGridSize {  get { return unityGridSize; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public Dictionary<Vector2Int, Node> Grid {  get { return grid; } }

    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
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
            coord.Value.connectedTo = null;
            coord.Value.explored = false;
            coord.Value.path = false;
        }
    }

    public Vector2Int GetCoordsFromPos(Vector3 pos)
    {
        Vector2Int coords = new Vector2Int();

        coords.x = Mathf.RoundToInt(pos.x / UnityGridSize);
        coords.y = Mathf.RoundToInt(pos.z / UnityGridSize);

        return coords;
    }

    public Vector3 GetPosFromCoords(Vector2Int coords, float y)
    {
        Vector3 pos = new Vector3();

        pos.x = coords.x * UnityGridSize;
        pos.y = y;
        pos.z = coords.y * UnityGridSize;

        return pos;
    }

    private void CreateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (!premade)
                {
                    var obj = Instantiate(tiles[Random.Range(0, tiles.Length)], new Vector3(x * unityGridSize, 0, y * unityGridSize), Quaternion.identity);
                    var netObj = obj.GetComponent<NetworkObject>();
                    netObj.Spawn();
                }
            }
        }
    }

    private void OnServerStarted()
    {
        CreateGrid();
    }
}
