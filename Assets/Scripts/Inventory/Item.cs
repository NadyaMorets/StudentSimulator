using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public abstract Stats stats { get; }
}
