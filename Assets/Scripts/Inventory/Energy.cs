using System;
using UnityEngine;

[Serializable]
public class Energy : Item
{
    public override Stats stats => new Stats()
    {
        EnergyPercent = 5
    };
}
