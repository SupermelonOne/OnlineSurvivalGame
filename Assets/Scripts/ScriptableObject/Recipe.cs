using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Recipe/Recipe")] //either set variable to required station or make different types for different stations
public class Recipe : ScriptableObject
{
    public string recipeName;
    public List<RecipePart> results = new List<RecipePart>();
    public List<RecipePart> requirements = new List<RecipePart>();
}
