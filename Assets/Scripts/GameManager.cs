using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
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
    UnitController unitController;

    UIDocument doc;
    public VisualElement ui;
    public Button nextTurnButton;
    public Button ability1Button;
    public Button ability2Button;

    public NetworkVariable<bool> p1Turn = new NetworkVariable<bool>(true);

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        gridManager = GridManager.Instance;
        unitController = UnitController.Instance;

        doc = FindFirstObjectByType<UIDocument>();
        doc.rootVisualElement.schedule.Execute(() =>
        {
            ui = doc.rootVisualElement;

            nextTurnButton = ui.Q<Button>("EndTurnButton");
            ability1Button = ui.Q<Button>("Ability1Button");
            ability2Button = ui.Q<Button>("Ability2Button");
        }).ExecuteLater(0);

        p1Turn.OnValueChanged += (oldVal, newVal) =>
        {
            NextTurn();
        };
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
        NextTurn();
        SetButtonText();
    }

    [Rpc(SendTo.Server)]
    public void SwapTurnRpc()
    {
        List<Unit> army;
        if (p1Turn.Value) army = playerTwoArmy;
        else army = playerOneArmy;

        if(army.Count <= 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        foreach (Unit unit in army)
        {
            if (unit.GetPoisonedTurns() > 0)
            {
                unit.SetHealthRpc(unit.GetHealth() - Mathf.RoundToInt(unit.GetHealth() / 10));
                unit.SetPoisonedTurnsRpc(unit.GetPoisonedTurns() - 1);
                unit.CheckDeathRpc();
            }
        }
        if (army.Count <= 0)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        foreach (Unit unit in army)
        {
            if (unit.GetStunnedTurns() > 0)
            {
                unit.SetRemainingSpeedRpc(0);
                unit.SetUsedAbilityRpc(true);
                unit.SetStunnedTurnsRpc(unit.GetStunnedTurns() - 1);
            }
            else
            {
                unit.SetRemainingSpeedRpc(unit.GetGridSpeed());
                unit.SetUsedAbilityRpc(false);
            }
            unit.selectedAbility = null;
            unit.abilityID = -1;
        }
        p1Turn.Value = !p1Turn.Value;
    }

    void NextTurn()
    {
        if ((p1Turn.Value && IsHost) || (!p1Turn.Value && !IsHost))
        {
            nextTurnButton.SetEnabled(true);
            nextTurnButton.text = "End Your Turn";
        }
        else
        {
            nextTurnButton.SetEnabled(false);
            nextTurnButton.text = "Wait For Your Turn";
        }
        unitController.selectedUnit = null;
        unitController.unitSelected = false;
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

    public bool SetSelectedAbility(int abilityID)
    {
        if(unitController.selectedUnit != null)
        {
            unitController.selectedUnit.selectedAbility = unitController.selectedUnit.GetAbilityFromID(abilityID);
            Debug.Log(unitController.selectedUnit.selectedAbility.abilityName);
            return true;
        }
        else
        {
            Debug.Log("No unit selected.");
            return false;
        }        
    }
    
    public void SetButtonText()
    {
        if(unitController.selectedUnit != null)
        {
            ability1Button.text = unitController.selectedUnit.GetAbilityFromID(0).abilityName;
            ability2Button.text = unitController.selectedUnit.GetAbilityFromID(1).abilityName;
            ability1Button.SetEnabled(true);
            ability2Button.SetEnabled(true);
        }
        else
        {
            ability1Button.text = "";
            ability2Button.text = "";
            ability1Button.SetEnabled(false);
            ability2Button.SetEnabled(false);
        }
    }
}
