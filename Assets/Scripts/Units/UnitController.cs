using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class UnitController : NetworkBehaviour
{
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
                            RecalculatePathRpc(true, startCoords, targetCoords, selectedUnit.transform.position, NetworkManager.IsHost);
                        }
                        break;
                    case "Player1":
                    case "Player2":
                        if(selectedUnit != null && selectedUnit.tag != hit.transform.tag)
                        {
                            pathFinder.SetNewDestination(gridManager.GetCoordsFromPos(selectedUnit.transform.position), gridManager.GetCoordsFromPos(hit.transform.position));
                            selectedUnit.selectedAbility.Effect(selectedUnit, hit.transform.GetComponent<Unit>(), pathFinder.GetNewPath().Count - 1);
                        }
                        if((IsHost && hit.transform.CompareTag("Player1")) || (!IsHost && hit.transform.CompareTag("Player2")))
                        {
                            selectedUnit = hit.transform.GetComponent<Unit>();
                            unitSelected = true;
                        }
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
    void RecalculatePathRpc(bool resetPath, Vector2Int startCoords, Vector2Int targetCoords, Vector3 unitPos, bool host)
    {
        FindUnit(unitPos, host);

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
        if (selectedUnit.remainingSpeed < path.Count - 1)
        {
            return;
        }
        StartCoroutine(FollowPath());
    }

    private void FindUnit(Vector3 unitPos, bool host)
    {
        Collider[] results = Physics.OverlapSphere(unitPos, 0.25f);
        foreach(Collider result in results)
        {
            try
            {
                if((host && result.CompareTag("Player1")) || (!host && result.CompareTag("Player2")))
                {
                    selectedUnit = result.GetComponent<Unit>();
                    break;
                }
            }
            catch(Exception)
            {
                continue;
            }
        }
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
                travelPercent += Time.deltaTime * selectedUnit.movementSpeed;
                selectedUnit.transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }
        }
        selectedUnit.remainingSpeed -= path.Count;
        gridManager.Grid[gridManager.GetCoordsFromPos(selectedUnit.transform.position)].walkable = false;
        unitMoving = false;
    }
}
