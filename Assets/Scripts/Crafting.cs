using UnityEngine;

public class Crafting : MonoBehaviour
{
    //[SerializeField] RectTransform requirements;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] Recipe recipe;
    public void SetRecipe(Recipe recipe)
    {
        this.recipe = recipe;
    }
    public void SetPlayerInventory(PlayerInventory inventory)
    {
        this.inventory = inventory;
    }
    public void Craft()
    {
        inventory.CraftItem(RecipeDatas.Instance.GetIdByRecipe(recipe));
    }
}
