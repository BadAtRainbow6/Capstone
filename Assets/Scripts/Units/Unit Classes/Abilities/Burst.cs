using UnityEngine;

public class Burst : Ability
{
    public Burst()
    {
        abilityName = "Burst";
        maxCD = 3;
        curCD = 0;
        maxCharges = 1;
        curCharges = 1;
        range = 0;
    }

    public override bool Effect(Unit user, Unit target, int enemyRange)
    {
        user.SetGridSpeedRpc(user.GetGridSpeed() + 3);
        Debug.Log(abilityName + " went off.");
        return true;
    }
}