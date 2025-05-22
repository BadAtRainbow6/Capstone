using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Unit> playerOneChosen = new List<Unit>();
    public List<Unit> playerTwoChosen = new List<Unit>();

    [HideInInspector] public List<Unit> playerOneArmy = new List<Unit>();
    [HideInInspector] public List<Unit> playerTwoArmy = new List<Unit>();

    public List<Transform> pOneSpawnPoints = new List<Transform>();
    public List<Transform> pTwoSpawnPoints = new List<Transform>();

    GridManager gridManager;

    UIDocument doc;
    public VisualElement ui;
    public Button nextTurnButton;

    public bool p1Turn = true;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        gridManager = GridManager.Instance;
        doc = FindFirstObjectByType<UIDocument>();
        doc.rootVisualElement.schedule.Execute(() =>
        {
            ui = doc.rootVisualElement;

            nextTurnButton = ui.Q<Button>("EndTurnButton");
        }).ExecuteLater(0);
    }

    private void OnServerStarted()
    {
        if (IsHost)
        {
            for (int i = 0; i < playerOneChosen.Count; i++)
            {
                var obj = Instantiate(playerOneChosen[i], pOneSpawnPoints[i]);
                var netObj = obj.GetComponent<NetworkObject>();
                netObj.Spawn();

                obj.armyId = i;

                obj.GetComponent<Unit>().SetTagOnServer("Player1");
                playerOneArmy.Add(obj);
            }

            for (int i = 0; i < playerTwoChosen.Count; i++)
            {
                var obj = Instantiate(playerTwoChosen[i], pTwoSpawnPoints[i]);
                var netObj = obj.GetComponent<NetworkObject>();
                netObj.Spawn();

                obj.armyId = i;

                obj.GetComponent<Unit>().SetTagOnServer("Player2");
                playerTwoArmy.Add(obj);
            }

            StartCoroutine(DelayedBlocking());
        }
    }

    private void OnClientStarted()
    {
        NextTurnText();
    }

    public void SwapTurn()
    {
        List<Unit> army;
        if (p1Turn) army = playerTwoArmy;
        else army = playerOneArmy;

        if(army.Count <= 0)
        {
            Application.Quit();
        }
        foreach (Unit unit in new List<Unit>(army))
        {
            if (unit.statusTimer[Unit.Status.POISONED] > 0)
            {
                unit.health -= Mathf.RoundToInt(unit.health / 10);
                unit.statusTimer[Unit.Status.POISONED] -= 1;
                unit.CheckDeath();
            }
        }
        if (army.Count <= 0)
        {
            Application.Quit();
        }
        foreach(Unit unit in new List<Unit>(army))
        {
            if (unit.statusTimer[Unit.Status.STUNNED] > 0)
            {
                unit.remainingSpeed = 0;
                unit.usedAbility = true;
                unit.statusTimer[Unit.Status.STUNNED]--;
            }
            else
            {
                unit.remainingSpeed = unit.gridSpeed;
                unit.usedAbility = false;
            }
            unit.selectedAbility = null;
        }
        p1Turn = !p1Turn;
        NextTurnText();
    }

    void NextTurnText()
    {
        if ((p1Turn && IsHost) || (!p1Turn && !IsHost))
        {
            nextTurnButton.text = "End Your Turn";
        }
        else
        {
            nextTurnButton.text = "Wait For Your Turn";
        }
    }

    private IEnumerator DelayedBlocking()
    {
        // Wait one frame
        yield return null;

        // Now safe to proceed
        foreach (Unit unit in playerOneArmy)
        {
            gridManager.BlockNode(gridManager.GetCoordsFromPos(unit.transform.position));
        }
        foreach (Unit unit in playerTwoArmy)
        {
            gridManager.BlockNode(gridManager.GetCoordsFromPos(unit.transform.position));
        }
    }
}
