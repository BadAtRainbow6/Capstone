using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UnitController : NetworkBehaviour
{
    [SerializeField] float movementSpeed = 1f;

    Unit selectedUnit;
    bool unitSelected = false;

    bool unitMoving = false;

    List<Node> path = new List<Node>();

    GridManager gridManager;
    PathFinding pathFinder;

    public static UnitController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        pathFinder = FindFirstObjectByType<PathFinding>();
    }

    void Update()
    {
        PathFind();
    }

    void PathFind()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit && !unitMoving)
            {
                switch (hit.transform.tag)
                {
                    case "Terrain":
                        if (unitSelected)
                        {
                            Vector2Int targetCoords = hit.transform.GetComponent<Tile>().coords * gridManager.UnityGridSize;
                            Vector2Int startCoords = new Vector2Int((int)Mathf.Round(selectedUnit.transform.position.x), (int)Mathf.Round(selectedUnit.transform.position.z)) / gridManager.UnityGridSize;
                            RecalculatePathRpc(true, startCoords, targetCoords);
                        }
                        break;
                    case "Unit":
                        selectedUnit = hit.transform.GetComponent<Unit>();
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

    [Rpc(SendTo.Server)]
    void RecalculatePathRpc(bool resetPath, Vector2Int startCoords, Vector2Int targetCoords)
    {
        pathFinder.SetNewDestination(startCoords, targetCoords);

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
            Vector3 startPosition = selectedUnit.transform.position;
            Vector3 endPosition = gridManager.GetPosFromCoords(path[i].coords, selectedUnit.transform.position.y);

            float travelPercent = 0f;

            while(travelPercent < 1f)
            {
                travelPercent += Time.deltaTime * movementSpeed;
                selectedUnit.transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }
        }
        gridManager.Grid[gridManager.GetCoordsFromPos(selectedUnit.transform.position)].walkable = false;
        unitMoving = false;
    }
}
