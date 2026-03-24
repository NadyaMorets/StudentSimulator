using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField, SerializeReference, SubclassSelector]
    public Item item;
}
