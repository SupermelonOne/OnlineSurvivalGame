using System.ComponentModel;
using TMPro;
using UnityEngine;

public class SpawnRecipe : MonoBehaviour
{
    private Recipe myRecipe;
    private PlayerInventory myInventory;
    private int recipeId = 0;
    [SerializeField] GameObject requirementPrefab;
    [SerializeField] RectTransform requirementsOrigin;
    [SerializeField] TextMeshProUGUI resultName;
    [SerializeField] Crafting crafting;
    public void SetRecipe(Recipe recipe)
    {
        myRecipe = recipe;
    }
    public void SetPlayerInventory(PlayerInventory inventory)
    {
        myInventory = inventory;
    }
    public void Setup()
    {
        crafting.SetRecipe(myRecipe);
        crafting.SetPlayerInventory(myInventory);
        foreach(RecipePart part in myRecipe.requirements)
        {
            GameObject obj = Instantiate(requirementPrefab, requirementsOrigin);
            RequirementSetup rs = obj.GetComponent<RequirementSetup>();
            rs.SetTexture(part.item.displayTexture);
            rs.SetText(part.amount.ToString());
        }
        if (resultName != null)
        {
            resultName.text = myRecipe.recipeName;
        }
    }
    public void SetRecipeId(int id)
    {
        recipeId = id;
    }
}
