using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class InteractWithTile : NetworkBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Item equipedItem;
    [SerializeField] private int amountEquiped;

    [SerializeField] Texture emptyTexture;
    [SerializeField] RawImage itemSprite;
    [SerializeField] TextMeshProUGUI itemAmountText;
    private void Start()
    {
        SetItemAmountClientRpc(amountEquiped);
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetItemSpriteClientRpc(int itemId)
    {
        if (itemSprite == null)
            return;
        if (itemId == 0)
            itemSprite.texture = emptyTexture;
        else
            itemSprite.texture = ItemIds.Instance.GetItemById(itemId).displayTexture;
    }
    [ClientRpc(RequireOwnership = false)]
    private void SetItemAmountClientRpc(int amount)
    {
        Debug.Log("tried to update number");
        if (itemAmountText == null)
            return;
        if (amount == 0 || amount == 1)
        {
            itemAmountText.text = "";
            Debug.Log("emptyyy");
        }
        else
        {
            itemAmountText.text = amount.ToString();
            
        }
    }

    public void SetEquipedItem(int itemId, int amount)
    {
        if (itemId == 0)
            equipedItem = null;
        else
            equipedItem = ItemIds.Instance.GetItemById(itemId);
        amountEquiped = amount;
        SetItemSpriteClientRpc(itemId);
        SetItemAmountClientRpc(amount);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetEquipedItemServerRpc(int itemId, int amount)
    {
        equipedItem = ItemIds.Instance.GetItemById(itemId);
        amountEquiped = amount;
        SetItemSpriteClientRpc(itemId);
        SetItemAmountClientRpc(amount);
    }
    public int RemoveEquipedAmount()
    {
        int amount = amountEquiped;
        amountEquiped = 0;
        SetItemAmountClientRpc(amountEquiped);
        return amount;
    }
    public Item RemoveItem()
    {
        Item item = equipedItem;
        equipedItem = null;
        SetItemSpriteClientRpc(0);
        return item;
    }
    public bool HasItem()
    {
        return (equipedItem != null);
    }
    public void DestroyTile()
    {
        if (player == null)
            return;
        RaycastHit hit;
        if (Physics.Raycast(player.position, transform.position - player.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            int x = Mathf.FloorToInt(hit.point.x + .5f);
            int y = Mathf.FloorToInt(hit.point.z + .5f);
            if (TerrainGenerator.Instance.HasTile(x, y))
            {
                TerrainGenerator.Instance.ChangeTile(x, y, 0);
            }
            else
            {
                x = Mathf.FloorToInt(hit.collider.transform.position.x + .5f);
                y = Mathf.FloorToInt(hit.collider.transform.position.z + .5f);
                TerrainGenerator.Instance.ChangeTile(x, y, 0);
            }
        }

    }
    public void DamageTile()
    {
        if (player == null)
            return;
        int damageType = 0;
        if (equipedItem is Tool tool)
        {
            damageType = tool.toolType;
        }
        RaycastHit hit;
        if (Physics.Raycast(player.position, transform.position - player.position, out hit, Vector3.Distance(transform.position, player.position)))
        {
            int x = Mathf.FloorToInt(hit.point.x + .5f);
            int y = Mathf.FloorToInt(hit.point.z + .5f);
            if (!TerrainGenerator.Instance.HasTile(x, y))
            {
                x = Mathf.FloorToInt(hit.collider.transform.position.x + .5f);
                y = Mathf.FloorToInt(hit.collider.transform.position.z + .5f);
            }
            TerrainGenerator.Instance.HitTile(x, y, 1, damageType);

        }
        else
        {
            int x = Mathf.FloorToInt(transform.position.x + .5f);
            int y = Mathf.FloorToInt(transform.position.z + .5f);
            TerrainGenerator.Instance.HitTile(x, y, 1, damageType);
        }


    }

    public void PlaceTile()
    {
        if (equipedItem is not Placable placable) return;
        Vector3 direction = (transform.position - player.position).normalized;
        Vector3 targetPos = player.position + (direction * .4f);
        int x = Mathf.FloorToInt(player.transform.position.x + .5f);
        int y = Mathf.FloorToInt(player.transform.position.z + .5f);
        if (!TerrainGenerator.Instance.HasTile(x, y))
        {
            TerrainGenerator.Instance.ChangeTile(x, y, ObjectIds.Instance.GetIdByObject(placable.placableObject)); //stuur correcte id door
            amountEquiped--;
            if (amountEquiped <= 0)
            {
                amountEquiped = 0;
                equipedItem = null;
                SetItemSpriteClientRpc(0);
            }
            SetItemAmountClientRpc(amountEquiped);
        }
    }
}
