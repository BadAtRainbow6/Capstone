using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    GameManager manager;

    private void Start()
    {
        manager = FindFirstObjectByType<GameManager>();
        Instantiate(manager.playerOneArmy.First(), transform);
    }
}
