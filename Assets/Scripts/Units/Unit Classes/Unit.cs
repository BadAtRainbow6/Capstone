using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Collections.Generic;

public class Unit : NetworkBehaviour
{
    public enum Status
    {
        POISONED,
        STUNNED
    }

    [SerializeField] public float health = 10.0f;
    [SerializeField] public float gridSpeed = 3.0f; // How many tiles a unit can move in a turn. Will be shown as "Speed" in game.
    [SerializeField] public float movementSpeed = 1.0f; // How fast the unit moves from tile to tile in real time.
    [HideInInspector] public float remainingSpeed;

    public Dictionary<Status, int> statusTimer = new Dictionary<Status, int>();

    public bool flying;

    protected List<Ability> abilities = new List<Ability>();
    public Ability selectedAbility = null;
    public bool usedAbility = false;

    GameManager gameManager;

    private NetworkVariable<FixedString32Bytes> syncedTag = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected int id;
    int ID {  get { return id; } set { id = value; } }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        remainingSpeed = gridSpeed;
        statusTimer.Add(Status.POISONED, 0);
        statusTimer.Add(Status.STUNNED, 0);
    }

    public void CheckDeath()
    {
        if(health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (CompareTag("Player1"))
        {
            gameManager.playerOneArmy.Remove(this);
        }
        else
        {
            gameManager.playerTwoArmy.Remove(this);
        }
        Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (!string.IsNullOrEmpty(syncedTag.Value.ToString()))
        {
            gameObject.tag = syncedTag.Value.ToString();
        }
    }

    public void SetTagOnServer(string newTag)
    {
        if (IsServer)
        {
            syncedTag.Value = newTag;
            gameObject.tag = newTag;
        }
    }
}