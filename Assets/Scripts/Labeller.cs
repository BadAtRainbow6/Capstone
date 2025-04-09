using TMPro;
using UnityEngine;

[ExecuteAlways]
public class Labeller : MonoBehaviour
{
    TextMeshPro label;
    Vector2Int coords;
    GridManager gridManager;

    private void Awake()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        label = GetComponentInChildren<TextMeshPro>();

        if (!gridManager) { return; }

        coords.x = Mathf.RoundToInt(transform.position.x / gridManager.UnityGridSize);
        coords.y = Mathf.RoundToInt(transform.position.y / gridManager.UnityGridSize);

        label.text = $"{coords.x}, {coords.y}";
    }
}