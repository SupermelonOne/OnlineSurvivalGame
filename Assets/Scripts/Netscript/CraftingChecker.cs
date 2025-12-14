using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CraftingChecker
{
    public static bool CanCraft(PlayerInventory playerInventory, Recipe recipe)
    {
        bool canMake = true;
        List<RecipePart> requirements = new List<RecipePart>(recipe.requirements);
        foreach (RecipePart part in requirements)
        {
            if (!(playerInventory.GetAmountOfItem(part.item.id) >= part.amount))
            {
                canMake = false;
                break;
            }
        }
        return canMake;
    }
}
