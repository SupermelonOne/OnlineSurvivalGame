using NUnit.Framework;
using System.Collections.Generic;
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

                if (amountToAdd <= 0)
                    return;
            }
        }
        
        //DROP LEFTOVERS AGAIN
    }

    private void HandleItemChange(NetworkListEvent<int> change)
    {
        localItems[change.Index] = ItemIds.Instance.GetItemById(change.Value);
    }
    private void HandleAmountChange(NetworkListEvent<int> change)
    {
        localAmounts[change.Index] = change.Value;
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

    public void TakeFullSlot(int index)
    {
        if (localItems[0] == null)
        {
            localItems[0] = localItems[index];
            localItems[index] = null;
            localAmounts[0] = localAmounts[index];
            localAmounts[index] = 0;
        }
        else
        {
            Item replaceItem = localItems[index];
            int replaceAmount = localAmounts[index];
            localItems[index] = localItems[0];
            localAmounts[index] = localAmounts[0];
            localItems[0] = replaceItem;
            localAmounts[0] = replaceAmount;
        }
    }

}

//check of ( localAmounts + amount ) % item.Stackability < localAmounts % item.Stackability
//dat betekend dat een stack ge-overflowed heeft en de rest een nieuwe slot in moet
//maak ook een tackCheck die een bool terugsstuurd naar item op de grond of je al vol zit van het item
//of SPECEFIEK dat item nog ruimte heb, of gwn algemeen nog ruimte hebt
//zo niet moet die niet op jou attachen en achterna zitten
