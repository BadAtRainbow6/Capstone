using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;

    [SerializeField] bool premade;
    [SerializeField] GameObject[] tiles;
    public int UnityGridSize {  get { return unityGridSize; } }

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Dictionary<Vector2Int, Node> Grid {  get { return grid; } }

    private void Awake()
    {
        for (int x =  0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                grid.Add(coords, new Node(coords));

                if (!premade)
                {
                    Instantiate(tiles[Random.Range(0, tiles.Length - 1)], new Vector3(x * unityGridSize, 0, y * unityGridSize), Quaternion.identity);
                }
            }
        }
    }
}
