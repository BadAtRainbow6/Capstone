using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Unit;
using static UnityEngine.Rendering.DebugUI;

public class UnitController : NetworkBehaviour
{
    public Unit selectedUnit;
    public bool unitSelected = false;

    bool unitMoving = false;

    List<Node> path = new List<Node>();

    GridManager gridManager;
    PathFinding pathFinder;
    GameManager gm;

    public static UnitController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        pathFinder = FindFirstObjectByType<PathFinding>();
        gm = GameManager.Instance;
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

            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (hasHit && !unitMoving && ((gm.p1Turn.Value && IsHost) || (!gm.p1Turn.Value && !IsHost)))
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
                        if (selectedUnit != null && selectedUnit.tag != hit.transform.tag && !selectedUnit.GetUsedAbility())
                        {
                            pathFinder.SetNewDestination(gridManager.GetCoordsFromPos(selectedUnit.transform.position), gridManager.GetCoordsFromPos(hit.transform.position));
                            bool success = selectedUnit.selectedAbility.Effect(selectedUnit, hit.transform.GetComponent<Unit>(), pathFinder.GetNewPath(true).Count - 1);
                            if (success)
                            {
                                selectedUnit.PlayAbilityAnim(selectedUnit.abilityID);
                                selectedUnit.SetUsedAbilityRpc(true);
                            }
                        }
                        if ((IsHost && hit.transform.CompareTag("Player1")) || (!IsHost && hit.transform.CompareTag("Player2")))
                        {
                            selectedUnit = hit.transform.GetComponent<Unit>();
                            unitSelected = true;
                            gm.SetButtonText();
                        }
                        break;
                    default:
                        selectedUnit = null;
                        unitSelected = false;
                        gm.SetButtonText();
                        break;
                }
            }
            else if (!unitMoving)
            {
                selectedUnit = null;
                unitSelected = false;
                gm.SetButtonText();
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
        path = pathFinder.GetNewPath(coords, false);
        if (selectedUnit.GetRemainingSpeed() < path.Count - 1)
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
        selectedUnit.SetAnimState(AnimState.Walking);
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 startPosition = selectedUnit.transform.position;
            Vector3 endPosition = gridManager.GetPosFromCoords(path[i].coords, selectedUnit.transform.position.y);

            Vector3 lookDirection = (endPosition - startPosition).normalized;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                selectedUnit.transform.rotation = targetRotation;
            }

            float travelPercent = 0f;

            while(travelPercent < 1f)
            {
                travelPercent += Time.deltaTime * selectedUnit.GetMoveSpeed();
                selectedUnit.transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }
        }
        selectedUnit.SetRemainingSpeedRpc(selectedUnit.GetRemainingSpeed() - path.Count - 1);
        gridManager.Grid[gridManager.GetCoordsFromPos(selectedUnit.transform.position)].walkable = false;
        unitMoving = false;
        selectedUnit.SetAnimState(AnimState.Idle);
    }
}
