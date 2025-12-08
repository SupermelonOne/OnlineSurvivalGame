using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PickupableItem : NetworkBehaviour
{
    public Item item;
    NetworkVariable<int> itemId = new NetworkVariable<int>(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public int amount = 1;
    private PlayerInventory targetInventory = null;

    private List<Transform> checkedItems = new List<Transform>();

    Transform[] children;
    private Transform targetPlayer;

    private float speed = 0;
    private float maxSpeed = 3;

    //[SerializeField] private RawImage rawImage;
    MeshRenderer[] materials;

    [HideInInspector] public int priority;
    private void Start()
    {
        if (!IsServer)
            return;
        materials = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in materials)
        {
            meshRenderer.material = item.displaySprite;
        }
        children = GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        foreach (Transform child in children)
        {
            child.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
        }
        if (!IsServer)
            return;
        if (targetPlayer == null)
            return;

        transform.Translate((targetPlayer.position - transform.position).normalized * Time.deltaTime * speed, Space.World);
        if (speed < maxSpeed)
            speed += 1 * Time.deltaTime;
        else
            speed = maxSpeed;
        
        if (Vector3.Distance(targetPlayer.position, transform.position) < .5f)
        {
            targetInventory.AddItem(item, amount);
            DeleteItemClientRpc(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
            return;
        if (targetPlayer != null)
            return;
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory.HaveSpaceFor(item))
            {
                targetInventory = inventory;
                targetPlayer = other.transform;
            }
        }
        if (other.CompareTag("Item"))
        {
            if (checkedItems.Contains(other.transform))
                return;
            PickupableItem otherItem = other.GetComponent<PickupableItem>();
            if (otherItem.priority == priority)
            {
                if (otherItem.amount < amount)
                {
                    amount += otherItem.amount;
                    Destroy(otherItem.gameObject);
                }
                Debug.Log("same amount, remove this debugLog idc");
                checkedItems.Add(other.transform);
            }
        }
    }

    [ClientRpc] //HEADS UP. CHECK DE DOCUMENTATION, er is versie die iets niet naar server stuurt, wel naar clients, kan handig zijn
    private void DeleteItemClientRpc()
    {
        Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            if (itemId.Value != 0)
            {
                item = ItemIds.Instance.GetItemById(itemId.Value);
            }
            materials = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in materials)
            {
                meshRenderer.material = item.displaySprite;
            }
            children = GetComponentsInChildren<Transform>();
        }
        else
        {
            itemId.Value = item.id;
        }
    }
}
