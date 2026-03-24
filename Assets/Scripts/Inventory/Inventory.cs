using System.Collections.Generic;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public struct InventoryChangedArgument
    {
        public enum ChangeType
        {
            Add,
            Remove,
            Edit,
            Move
        }

        public Item item;
        public int index;
        public ChangeType eventType;
    }

    public CharacterInput player;

    public event Action<InventoryChangedArgument> onInventoryChanged;

    [SerializeField, SerializeReference]
    private List<Item> items = new();

    public void AddItem(Item item)
    {
        items.Add(item);

        player.stats = Stats.Sum(player.stats, item.stats);

        var eventArgs = new InventoryChangedArgument()
        {
            item = item,
            index = items.Count - 1,
            eventType = InventoryChangedArgument.ChangeType.Add
        };

        onInventoryChanged?.Invoke(eventArgs);
    }
    public void RemoveItem(Item item)
    {
        player.stats = Stats.Subtract(player.stats, item.stats);

        var eventArgs = new InventoryChangedArgument()
        {
            item = item,
            index = items.IndexOf(item),
            eventType = InventoryChangedArgument.ChangeType.Remove
        };

        items.Remove(item);

        onInventoryChanged?.Invoke(eventArgs);

    }
    public Item[] GetAllItems()
    {
        return items.ToArray();
    }
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    OnTriggerEnter(hit.collider);
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    OnTriggerEnter(collision.collider);
    //}
    //private void OnTriggerEnter(Collider collider)
    //{
    //    if (collider.TryGetComponent<ItemContainer>(out var container))
    //    {
    //        AddItem(container.item);
    //        //Destroy(container.gameObject);
    //    }
    //}
}
