using System.Collections.Generic;
using UnityEngine;

public class Paladin : Unit
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetRemainingSpeedRpc(GetGridSpeed());
        abilities.Add(new Smite());
        abilities.Add(new Bash());
    }
}

