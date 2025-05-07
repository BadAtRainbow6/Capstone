using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<Unit> playerOneArmy = new List<Unit>();
    public List<Unit> playerTwoArmy = new List<Unit>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerOneArmy.First().tag = "Player1";
        playerTwoArmy.First().tag = "Player2";
    }

    void Update()
    {
        
    }
}
