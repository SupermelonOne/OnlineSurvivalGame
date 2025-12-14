using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeDatas : MonoBehaviour
{
    [SerializeField] List<RecipeData> recipes = new List<RecipeData>();
    Dictionary<int, Recipe> activeRecipes = new Dictionary<int, Recipe>();
    Dictionary<Recipe, int> activeRecipesIds = new Dictionary<Recipe, int>();
    public static RecipeDatas Instance;
    private void Awake()
    {
        foreach(var recipe in recipes)
        {
            activeRecipes.Add(recipe.id, recipe.recipe);
            activeRecipesIds.Add(recipe.recipe, recipe.id);
        }
        Instance = this;
    }
    public List<RecipeData> GetRecipeDatas()
    {
        return recipes;
    }
    public Recipe GetRecipeById(int id)
    {
        return activeRecipes[id];
    }
    public int GetIdByRecipe(Recipe recipe)
    {
        return activeRecipesIds[recipe];
    }
    public int GetRecipeAmount()
    {
        return activeRecipes.Count;
    }
}
