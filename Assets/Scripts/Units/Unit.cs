using UnityEngine;
using Unity.Netcode;

public class Unit : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float gridSpeed; // How many tiles a unit can move in a turn. Will be shown as "Speed" in game.
    [SerializeField] float movementSpeed; // How fast the unit moves from tile to tile in real time.

    GameManager gameManager;

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
}