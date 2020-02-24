using System.Collections.Generic;
using UnityEngine;
using Zenject;
using LSW.Helpers;
using TMPro;

namespace Cooking.Recipe
{
    public class QuickRecipeLoader : MonoBehaviour, IRecipeLoader
    {
        [SerializeField] private Transform recipeContainer = null;
        
        [Space]
        [SerializeField] private TextMeshProUGUI titleName = null;

        [Inject]
        private QuickRecipeSlot.Factory _slotFactory = null;
        private List<IRecipeSlot> _slots;

        private RectTransform _window;
        private Vector2 _initialHeight;

        private void Awake()
        {
            _slots = new List<IRecipeSlot>();
            _window = GetComponent<RectTransform>();
            _initialHeight = _window.sizeDelta;
        }

        public void FetchRecipes(string label)
        {
            titleName.text = label;
            ClearItems();

            var recipes = RecipeManager.Recipes;
            var style = RecipeManager.Style;
            foreach (var recipe in recipes)
            {
                if (style == RecipeManager.RecipeStyle.FoodCooking && recipe.itemsRequired.Count > 1)
                    continue;
                
                _slots.Add(_slotFactory.Create(recipeContainer));
                _slots[_slots.Count - 1].SetRecipe(recipe).CheckRequirement();
            }

            AdjustWindowSize(_slots.Count);
        }

        private void AdjustWindowSize(int recipesCount)
        {
            int rows = recipesCount / 7;
            var newHeight = _initialHeight;
            newHeight.y += rows * 0.8f;
            _window.sizeDelta = newHeight;
        }

        private void ClearItems()
        {
            _slots.Clear();
            for (var i = 0; i < recipeContainer.childCount; i++)
                Destroy(recipeContainer.GetChild(i).gameObject);
        }
    }
}