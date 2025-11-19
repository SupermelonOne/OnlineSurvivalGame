using UnityEngine;

public class TerrainObjectData : MonoBehaviour
{
    [SerializeField] private bool doShrinking = false;
    [SerializeField] private float health = 1;
    [SerializeField] private int healthType;
    private float MaxHealth;
    private void Start()
    {
        MaxHealth = health;
    }
    public bool TakeDamage(float damage, int damageType)
    {
        if (damageType == healthType)
        {
            Debug.Log("correct tool");
            health -= damage;
        }
        else
        {
            health -= damage / 4;
        }

        if (doShrinking)
        {
            float size = health / MaxHealth;
            transform.localScale = new Vector3(size, size, size);
        }

        return (health <= 0);
    }
}
