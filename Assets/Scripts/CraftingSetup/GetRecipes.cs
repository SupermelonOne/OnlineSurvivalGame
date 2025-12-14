using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GetRecipes : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] GameObject recipePrefab;
    [SerializeField] RectTransform recipeHolder;
    private void Start()
    {
        List<RecipeData> allRecipeDatas = new List<RecipeData>(RecipeDatas.Instance.GetRecipeDatas());
        foreach (RecipeData data in allRecipeDatas)
        {
            Recipe recipe = data.recipe;
            GameObject newRecipeObj = Instantiate(recipePrefab, recipeHolder);
            SpawnRecipe spawnRecipe = newRecipeObj.GetComponent<SpawnRecipe>();
            spawnRecipe.SetRecipe(recipe);
            spawnRecipe.SetRecipeId(data.id);
            spawnRecipe.SetPlayerInventory(inventory);
            spawnRecipe.Setup();
        }
    }
}
