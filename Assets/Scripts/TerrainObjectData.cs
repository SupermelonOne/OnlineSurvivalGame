using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainObjectData : MonoBehaviour
{
    [SerializeField] private bool doShrinking = false;
    [SerializeField] private float health = 1;
    [SerializeField] private List<int> healthType;
    [SerializeField] private float wrongToolDebuff = 4f;

    //list of items to drop on hit, and chance of this happening
    [SerializeField] private List<ItemDrop> hitDrops = new List<ItemDrop>();
    [SerializeField] private List<ItemDrop> detroyDrop = new List<ItemDrop>();
    //list of items dropped on destroy anc chance of this happening
    private float MaxHealth;
    private void Start()
    {
        MaxHealth = health;
        if (healthType.Count == 0)
            healthType.Add(0);
    }
    public bool TakeDamage(float damage, int damageType)
    {
        if (healthType.Contains(damageType))
        {
            Debug.Log("correct tool");
            health -= damage;
        }
        else
        {
            health -= damage / wrongToolDebuff;
        }

        if (doShrinking)
        {
            float size = ((health / MaxHealth) * 0.8f) + 0.2f;
            transform.localScale = new Vector3(size, size, size);
        }

        return (health <= 0);
    }

    public List<Item> CheckHitDrops(int damageType)
    {
        List<Item> returnList = new List<Item>();
        foreach(ItemDrop itemDrop in hitDrops)
        {
            if (!itemDrop.needsCorrectTool || healthType.Contains(damageType))
            {
                if (Random.Range(0f, 1f) < itemDrop.chance)
                {
                    returnList.Add(itemDrop.item);
                }
            }
        }
        return returnList;
    }

    
}
