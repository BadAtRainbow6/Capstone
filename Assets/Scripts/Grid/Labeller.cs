using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Labeller : MonoBehaviour
{
    public Vector2Int coords;
    GridManager gridManager;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        DisplayCoords();
    }

    private void Update()
    {
        DisplayCoords();
        transform.name = coords.ToString();
    }

    private void DisplayCoords()
    {
        if (!gridManager) { return; }

        coords.x = Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize);
        coords.y = Mathf.RoundToInt(transform.position.z / gridManager.UnityGridSize);
    }
}