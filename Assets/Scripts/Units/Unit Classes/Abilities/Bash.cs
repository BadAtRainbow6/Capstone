using UnityEngine;

public class Bash : Ability
{
    public Bash()
    {
        abilityName = "Bash";
        maxCD = 4;
        curCD = 0;
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
        target.SetHealthRpc(target.GetHealth() - 3);
        target.SetStunnedTurnsRpc(target.GetStunnedTurns() + 1);
        target.CheckDeathRpc();
        Debug.Log(abilityName + " went off.");
        return true;
    }
}
