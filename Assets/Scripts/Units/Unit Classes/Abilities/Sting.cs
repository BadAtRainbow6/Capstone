using UnityEngine;

public class Sting : Ability
{
    public Sting()
    {
        abilityName = "Sting";
        maxCD = 1;
        curCD = 1;
        maxCharges = 1;
        curCharges = 1;
        range = 1;
    }

    public override bool Effect(Unit user, Unit target, int enemyRange)
    {
        if (enemyRange > range || enemyRange == 0)
        {
            Debug.Log("Invalid Target");
            return false;
        }
        target.SetHealthRpc(target.GetHealth() - 4);
        target.CheckDeathRpc();
        Debug.Log(abilityName + " went off.");
        return true;
    }
}
