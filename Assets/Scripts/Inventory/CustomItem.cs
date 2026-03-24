using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[Serializable]
public class CustomItem : Item
{
    [SerializeField]
    public Stats _stats;

    public override Stats stats => _stats;
}
