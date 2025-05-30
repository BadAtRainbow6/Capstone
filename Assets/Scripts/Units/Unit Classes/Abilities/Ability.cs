using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string abilityName { get; protected set; }

    protected int maxCD { get; set; }
    protected int curCD { get; set; }
    protected int range { get; set; }
    protected int maxCharges { get; set; }
    protected int curCharges { get; set; }

    public abstract bool Effect(Unit user, Unit target, int enemyRange);
}
