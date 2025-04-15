using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int coords;

    GridManager gridManager;

    [SerializeField] bool blocked;

    void Start()
    {
        SetCoords();
        gridManager.Grid.Add(coords, new Node(coords, blocked));

        if(blocked)
        {
            gridManager.BlockNode(coords);
        }
    }

    private void Update()
    {
        if (blocked && gridManager.Grid[coords].walkable)
        {
            gridManager.BlockNode(coords);
        }
    }

    private void SetCoords()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        int x = (int)transform.position.x;
        int z = (int)transform.position.z;

        coords = new Vector2Int(x / gridManager.UnityGridSize, z / gridManager.UnityGridSize);
    }
}
