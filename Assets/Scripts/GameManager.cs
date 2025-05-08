using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Unit> playerOneChosen = new List<Unit>();
    public List<Unit> playerTwoChosen = new List<Unit>();

    [HideInInspector] public List<Unit> playerOneArmy = new List<Unit>();
    [HideInInspector] public List<Unit> playerTwoArmy = new List<Unit>();

    public List<Transform> pOneSpawnPoints = new List<Transform>();
    public List<Transform> pTwoSpawnPoints = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
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

                obj.GetComponent<Unit>().SetTagOnServer("Player1");
                playerOneArmy.Add(obj);
            }

            for (int i = 0; i < playerTwoChosen.Count; i++)
            {
                var obj = Instantiate(playerTwoChosen[i], pTwoSpawnPoints[i]);
                var netObj = obj.GetComponent<NetworkObject>();
                netObj.Spawn();

                obj.GetComponent<Unit>().SetTagOnServer("Player2");
                playerTwoArmy.Add(obj);
            }
        }
    }
}
