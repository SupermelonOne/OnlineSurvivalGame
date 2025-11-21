using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;

    public GameObject placableObject; //should be checked if the object you input is a terrain object (with editor)
    public bool canPlaceAsObject; //should be automated with a proper editor script

    public Material displaySprite;
    //public Texture displaySprite;
}
