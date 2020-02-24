using UnityEngine;

namespace Cooking.Recipe
{
    [CreateAssetMenu(fileName = "RecipeType", menuName = "Cooking/Recipe Type")]
    public class RecipeType : ScriptableObject
    {
        public RecipeManager.RecipeStyle recipeStyle;
        public string autoButtonName;
        public string manualButtonName;
    }
}