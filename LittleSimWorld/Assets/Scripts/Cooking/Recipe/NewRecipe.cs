using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using System.Linq;

namespace Cooking
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Cooking/New recipe")]
    public class NewRecipe : ScriptableObject
    {
        [Header("Recipe:")]
        public ItemCode RecipeOutcome;
        
        [Space, Header("Ingredients:")]
        public List<ItemCode> itemsRequired;

        [Space, Header("Other Stuff")]
        public float timeToCook = 20f;
        public int EXPAwarded;

        public bool IsMatch(List<ItemCode> items)
        {
            if (items.Count != itemsRequired.Count)
                return false;

            foreach (var itemRequired in itemsRequired)
                if (itemsRequired.Count(i => i == itemRequired) != items.Count(i => i == itemRequired))
                    return false;

            return true;
        }
    }
}