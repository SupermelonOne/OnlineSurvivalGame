using System.Collections.Generic;
using UnityEngine;

public class ItemIds : MonoBehaviour
{
    [SerializeField] List<ItemId> itemIds = new List<ItemId>();
    Dictionary<int, Item> activeItemIds = new Dictionary<int, Item>();
    public static ItemIds Instance;
    private void Awake()
    {
        foreach (ItemId item in itemIds)
        {
            activeItemIds.Add(item.id, item.item);
        }
        Instance = this;
    }
    public Item GetItemById(int id)
    {
        return activeItemIds[id];
    }
    public int GetItemAmount()
    {
        return activeItemIds.Count;
    }
}
