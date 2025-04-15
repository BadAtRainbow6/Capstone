using Unity.VisualScripting;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject unit;

    private void Start()
    {
        Instantiate(unit, transform);
    }
}
