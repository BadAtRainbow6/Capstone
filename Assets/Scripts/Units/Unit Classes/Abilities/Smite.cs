using UnityEngine;

public class Smite : Ability
{
    public Smite()
    {
        abilityName = "Smite";
        maxCD = 1;
        curCD = 1;
        maxCharges = 1;
        curCharges = 1;
        range = 1;
    }

    public override bool Effect(Unit user, Unit target, int enemyRange)
    {
        if (target.flying == true || enemyRange > range || enemyRange == 0)
        {
            Debug.Log("Invalid Target");
            return false;
        }
        target.SetHealthRpc(target.GetHealth() - 100);
        target.CheckDeathRpc();
        Debug.Log(abilityName + " went off.");
        return true;
    }
}