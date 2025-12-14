using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public int stackability = 99;

    public Material displaySprite;
    public Texture displayTexture;
}
