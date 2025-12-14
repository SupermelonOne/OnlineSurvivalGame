using NUnit.Framework;
using SA.EventBusSystem;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;


public class PlayerInventory : NetworkBehaviour
{
    [SerializeField] private int inventorySize = 36;
    NetworkList<int> serverItems = new NetworkList<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    NetworkList<int> serverAmounts = new NetworkList<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);



    [SerializeField] private GameObject inventoryUI;
    [SerializeField] public List<Item> localItems = new List<Item>();
    [SerializeField] public List<int> localAmounts = new List<int>();
    [SerializeField] private List<int> totalAmounts = new List<int>();


    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }
    public override void OnNetworkSpawn()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            localItems.Add(null);
            localAmounts.Add(0);
        }
        if (!IsServer)
        {
            serverItems.OnListChanged += HandleItemChange;
            serverAmounts.OnListChanged += HandleAmountChange;
        }
        else
        {
            for (int i = 0; i < inventorySize; i++) //slot 0 shall be used for the cursor
            {
                serverItems.Add(0);
                serverAmounts.Add(0);
            }
        }
        for (int i = 0; i <= ItemIds.Instance.GetItemAmount(); i++)
        {
            totalAmounts.Add(0);
        }
    }
    public void AddItem(Item item, int amount)
    {
        if (!IsServer)
            return;

        int stackLimit = item.stackability;
        int amountToAdd = amount;

        for (int i = 1; i < inventorySize; i++)
        {
            if (localItems[i] == item)
            {
                int currentAmount = localAmounts[i];
                if (currentAmount < stackLimit)
                {
                    int spaceFree = stackLimit - currentAmount;
                    int add = Mathf.Min(spaceFree, amountToAdd);

                    localAmounts[i] += add;
                    amountToAdd -= add;

                    //server sided variable updated
                    serverAmounts[i] += add;

                    totalAmounts[item.id] += add;
                    InventoryEventBus<ItemCollected>.Publish(new ItemCollected(i));

                    if (amountToAdd <= 0)
                        return; // everything is added
                    //otherwise loop through with the resulting amount left
                }
            }
        }

        for (int i = 1; i < inventorySize; i++)
        {
            if (localItems[i] == null)
            {
                int add = Mathf.Min(stackLimit, amountToAdd);

                localItems[i] = item;
                localAmounts[i] = add;

                //server sided variables updated
                serverItems[i] = item.id;
                serverAmounts[i] = add;

                amountToAdd -= add;

                InventoryEventBus<ItemCollected>.Publish(new ItemCollected(i));
                totalAmounts[item.id] += add;

                if (amountToAdd <= 0)
                    return;
            }
        }

        //DROP LEFTOVERS AGAIN

        //MAKE INVENTORY SLOTS UPDATE IF YOU ARE THE HOST AS WELL
    }
    //Remove Item (and maybe add item as well idk rn, timeline crunch) have to be rewritten and the server than also subscribes on the list changes
    public void RemoveItem(Item item, int amount) //maybe change this to bool checking if it was able to remove enough items
    {
        if (!IsServer) return;
        int itemId = item.id;
        int amountToRemove = amount;

        for (int i = 0; i < inventorySize; i++)
        {
            if (serverItems[i] == itemId)
            {
                if (serverAmounts[i] > amountToRemove)
                {
                    serverAmounts[i] -= amountToRemove;
                    localAmounts[i] -= amountToRemove;
                    InventoryEventBus<ItemCollected>.Publish(new ItemCollected(i));

                    break;
                }
                else
                {
                    amountToRemove -= localAmounts[i];
                    serverAmounts[i] = 0;
                    localAmounts[i] = 0;
                    localItems[i] = null;
                    serverItems[i] = 0;
                    InventoryEventBus<ItemCollected>.Publish(new ItemCollected(i));

                }
            }
        }

    }

    private void HandleItemChange(NetworkListEvent<int> change)
    {
        if (change.Value == 0)
        {
            localItems[change.Index] = null;
            return;
        }
        localItems[change.Index] = ItemIds.Instance.GetItemById(change.Value);
        if (IsOwner)
        {
            InventoryEventBus<ItemCollected>.Publish(new ItemCollected(change.Index));
        }
    }
    private void HandleAmountChange(NetworkListEvent<int> change)
    {
        localAmounts[change.Index] = change.Value;
        if (IsOwner)
        {
            InventoryEventBus<ItemCollected>.Publish(new ItemCollected(change.Index));
        }
    }

    public bool HaveSpaceFor(Item item)
    {
        //check first for empty slots, since this is more likely to be true
        for (int i = 1; i < inventorySize; i++)
        {
            if (localItems[i] == null)
            {
                return true;
            }
        }
        int stackLimit = item.stackability;
        for (int i = 1; i < inventorySize; i++)
        {
            if (localItems[i] == item)
            {
                int currentAmount = localAmounts[i];
                if (currentAmount < stackLimit)
                {
                    return true;
                }
            }
        }
        return false;
    }
    int previousValue = 0;
    public void TakeFullSlot(int index, bool UpdateNet = true)
    {
        if (localItems[0] == null)
        {
            localItems[0] = localItems[index];
            localItems[index] = null;
            localAmounts[0] = localAmounts[index];
            localAmounts[index] = 0;
            if (UpdateNet)
                ChangeItemSlotServerRpc(index, 0);

            previousValue = index;
        }
        else
        {
            Item replaceItem = localItems[index];
            int replaceAmount = localAmounts[index];
            localItems[index] = localItems[0];
            localAmounts[index] = localAmounts[0];
            localItems[0] = replaceItem;
            localAmounts[0] = replaceAmount;

            //send moved info over
            if (UpdateNet)
                ChangeItemSlotServerRpc(0, index);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeItemSlotServerRpc(int slot1, int slot2)
    {
        if (!IsServer) return;
        //Debug.Log("banana");
        int keepItem = serverItems[slot1];
        int keepAmount = serverAmounts[slot1];
        serverItems[slot1] = serverItems[slot2];
        serverAmounts[slot1] = serverAmounts[slot2];
        serverItems[slot2] = keepItem;
        serverAmounts[slot2] = keepAmount;
        if (!IsOwner)
        {
            TakeFullSlot(slot1, false);
            TakeFullSlot(slot2, false);
        }
    }
    public int GetAmountOfItem(int itemId)
    {
        int amount = 0;
        for (int i = 0; i < inventorySize; i++)
        {

            if (localItems[i] != null && localItems[i].id == itemId)
            {
                amount += localAmounts[i];
            }
        }
        return amount;
    }
    //this is all server side checking
    public int GetAmountOfItem(Item item)
    {
        return GetAmountOfItem(item.id);
    }
    public void CraftItem(int reciptId)
    {
        OnlineCraftServerRpc(reciptId);
    }
    [ServerRpc]
    public void OnlineCraftServerRpc(int reciptId)
    {
        if (!IsServer)
            return;
        if (CraftingChecker.CanCraft(this, RecipeDatas.Instance.GetRecipeById(reciptId)))
        {
            HandleCraftingServerRpc(reciptId);
        }
    }
    [ServerRpc(RequireOwnership = false)] //MAKE THIS SERVER ONLY LATER
    private void HandleCraftingServerRpc(int recipeId)
    {
        if (!IsServer)
            return;
        List<RecipePart> requirements = new List<RecipePart>(RecipeDatas.Instance.GetRecipeById(recipeId).requirements);
        foreach (RecipePart part in requirements)
        {
            RemoveItem(part.item, part.amount);
        }
        List<RecipePart> results = new List<RecipePart>(RecipeDatas.Instance.GetRecipeById(recipeId).results);
        foreach(RecipePart part in results)
        {
            AddItem(part.item, part.amount);
        }

    }

    /*    public bool GetAmountOfItem(int itemId, int amount)
        {
            return (totalAmounts[itemId] >= amount);
        }
        public bool GetAmountOfItem(Item item, int amount)
        {
            return GetAmountOfItem(item.id, amount);
        }*/
}

