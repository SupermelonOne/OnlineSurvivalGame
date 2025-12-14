using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipItem : NetworkBehaviour
{
    [SerializeField] InteractWithTile interactWithTile;
    [SerializeField] PlayerInventory playerInventory;

    [ServerRpc(RequireOwnership = false)]
    public void EquipServerRpc()
    {
        if (playerInventory.localItems[0] != null)
        {
            if (interactWithTile.HasItem())
            {
                Item switchItem = interactWithTile.RemoveItem();
                int switchAmount = interactWithTile.RemoveEquipedAmount();
                interactWithTile.SetEquipedItemServerRpc(playerInventory.localItems[0].id, playerInventory.localAmounts[0]);

                EquipClientRpc(playerInventory.localItems[0].id, playerInventory.localAmounts[0]);
                if (IsServer)
                {
                    playerInventory.RemoveItem(playerInventory.localItems[0], playerInventory.localAmounts[0]);
                    playerInventory.AddItem(switchItem, switchAmount);
                }
            }
            else
            {
                interactWithTile.SetEquipedItemServerRpc(playerInventory.localItems[0].id, playerInventory.localAmounts[0]);

                EquipClientRpc(playerInventory.localItems[0].id, playerInventory.localAmounts[0]);
                if (IsServer) 
                    playerInventory.RemoveItem(playerInventory.localItems[0], playerInventory.localAmounts[0]);
            }
        }
        else
        {
            if (interactWithTile.HasItem())
            {
                Item switchItem = interactWithTile.RemoveItem();
                if (switchItem == null)
                    return;
                int switchAmount = interactWithTile.RemoveEquipedAmount();
                EquipClientRpc(0, 0);
                if (IsServer)
                {
                    playerInventory.AddItem(switchItem, switchAmount);
                }
            }
        }
    }
    [ClientRpc(RequireOwnership =false)]
    public void EquipClientRpc(int itemId, int amount)
    {

    }
}
