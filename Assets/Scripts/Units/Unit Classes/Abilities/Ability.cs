using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    protected int maxCD { get; set; }
    protected int curCD { get; set; }
    protected int range { get; set; }
    protected int charges { get; set; }

    public abstract bool Effect(Unit user, Unit target, int enemyRange);
}
