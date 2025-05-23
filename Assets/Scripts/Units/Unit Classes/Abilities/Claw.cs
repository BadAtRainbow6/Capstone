using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class Claw : Ability
{
    public Claw()
    {
        abilityName = "Claw";
        maxCD = 1;
        curCD = 1;
        maxCharges = 1;
        curCharges = 1;
        range = 1;
    }

    public override bool Effect(Unit user, Unit target, int enemyRange)
    {
        if (target.flying == true || enemyRange > range)
        {
            print("Invalid Target");
            return false;
        }
        target.health -= 8;
        target.CheckDeath();
        Debug.Log(abilityName);
        return true;
    }
}
