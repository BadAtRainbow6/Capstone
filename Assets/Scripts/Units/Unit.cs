using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class Unit : NetworkBehaviour
{
    [SerializeField] float health;
    [SerializeField] float gridSpeed; // How many tiles a unit can move in a turn. Will be shown as "Speed" in game.
    [SerializeField] float movementSpeed; // How fast the unit moves from tile to tile in real time.

    GameManager gameManager;

    private NetworkVariable<FixedString32Bytes> syncedTag = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected int id;
    int ID {  get { return id; } set { id = value; } }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
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
        Destroy(this);
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