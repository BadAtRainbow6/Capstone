using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;

    Transform selectedUnit;
    bool unitSelected = false;

    GridManager gridManager;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                switch(hit.transform.tag)
                {
                    case "Terrain":
                        if(unitSelected)
                        {
                            Vector2Int targetCoords = hit.transform.GetComponent<Labeller>().coords;
                            Vector2Int startCoords = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.z) / gridManager.UnityGridSize;

                            selectedUnit.transform.position = new Vector3(targetCoords.x, selectedUnit.position.y, targetCoords.y);
                        }
                        break;
                    case "Unit":
                        selectedUnit = hit.transform;
                        unitSelected = true;
                        break;
                    default:
                        selectedUnit = null;
                        unitSelected = false;
                        break;
                }
            }
            else
            {
                selectedUnit = null;
                unitSelected = false;
            }
        }
    }
}
