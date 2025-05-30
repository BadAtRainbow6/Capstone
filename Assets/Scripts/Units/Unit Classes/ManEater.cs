using System.Collections.Generic;
using UnityEngine;

public class ManEater : Unit
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetRemainingSpeedRpc(GetGridSpeed());
        abilities.Add(new Claw());
        abilities.Add(new Poison());
    }
}
