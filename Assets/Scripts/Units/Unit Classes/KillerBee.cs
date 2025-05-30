using System.Collections.Generic;
using UnityEngine;

public class KillerBee : Unit
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetRemainingSpeedRpc(GetGridSpeed());
        abilities.Add(new Sting());
        abilities.Add(new Burst());
    }
}
