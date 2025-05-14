using System.Collections.Generic;
using UnityEngine;

public class ManEater : Unit
{
    private void Start()
    {
        remainingSpeed = 8;
        abilities.Add(new Claw());
        selectedAbility = abilities[0];
    }
}
