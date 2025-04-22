using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] float gridSpeed; // How many tiles a unit can move in a turn. Will be shown as "Speed" in game.
    [SerializeField] float movementSpeed; // How fast the unit moves from tile to tile in real time.

    protected int id;
    int ID {  get { return id; } set { id = value; } }

    Player owner;

    void Start()
    {

    }

    void Update()
    {
        
    }
}