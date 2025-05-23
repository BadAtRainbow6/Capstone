using System.Collections.Generic;
using UnityEngine;

public class ManEater : Unit
{
    private void Start()
    {
        remainingSpeed = gridSpeed;
        abilities.Add(new Claw());
        abilities.Add(new Poison());
    }
}
