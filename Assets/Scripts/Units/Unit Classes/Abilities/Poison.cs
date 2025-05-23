using UnityEngine;

public class Poison : Ability
{
    public Poison()
    {
        abilityName = "Poison";
        maxCD = 5;
        curCD = 0;
        maxCharges = 2;
        curCharges = 2;
        range = 3;
    }

    public override bool Effect(Unit user, Unit target, int enemyRange)
    {
        if(enemyRange > range || (curCD > 0 && curCharges <= 0))
        {
            Debug.Log("Invalid Target");
            return false;
        }
        target.statusTimer[Unit.Status.POISONED] += 3;
        Debug.Log(abilityName);
        return true;
    }
}