using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;

    Transform selectedUnit;
    bool unitSelected = false;

    bool unitMoving = false;

    List<Node> path = new List<Node>();

    GridManager gridManager;
    PathFinding pathFinder;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        pathFinder = FindFirstObjectByType<PathFinding>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit && !unitMoving)
            {
                switch(hit.transform.tag)
                {
                    case "Terrain":
                        if(unitSelected)
                        {
                            Vector2Int targetCoords = hit.transform.GetComponent<Tile>().coords * gridManager.UnityGridSize;
                            Vector2Int startCoords = new Vector2Int((int)selectedUnit.position.x, (int)selectedUnit.position.z) / gridManager.UnityGridSize;
                            pathFinder.SetNewDestination(startCoords, targetCoords);
                            RecalculatePath(true);
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
            else if (!unitMoving)
            {
                selectedUnit = null;
                unitSelected = false;
            }
        }
    }

    void RecalculatePath(bool resetPath)
    {
        Vector2Int coords = new Vector2Int();
        if (resetPath)
        {
            coords = pathFinder.StartCoords;
        }
        else
        {
            coords = gridManager.GetCoordsFromPos(transform.position);
        }
        StopAllCoroutines();
        path.Clear();
        path = pathFinder.GetNewPath(coords);
        StartCoroutine(FollowPath());
    }

    IEnumerator<WaitForEndOfFrame> FollowPath()
    {
        unitMoving = true;
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 startPosition = selectedUnit.position;
            Vector3 endPosition = gridManager.GetPosFromCoords(path[i].coords);

            float travelPercent = 0f;

            while(travelPercent < 1f)
            {
                travelPercent += Time.deltaTime * movementSpeed;
                selectedUnit.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }
        }
        gridManager.Grid[gridManager.GetCoordsFromPos(selectedUnit.position)].walkable = false;
        unitMoving = false;
    }
}
