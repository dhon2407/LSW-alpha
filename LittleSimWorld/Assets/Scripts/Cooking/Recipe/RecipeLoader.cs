using UnityEngine;
using System.Collections.Generic;
using InventorySystem;
using LSW.Helpers;
using TMPro;
using Zenject;

namespace Cooking.Recipe
{
    public class RecipeLoader : MonoBehaviour, IRecipeLoader
    {
        [SerializeField] private Transform recipeContainer = null;
        [SerializeField] private CookList cookingList = null;
        [SerializeField] private CookingHandler handler = null;
        
        [Space]
        [SerializeField] private TextMeshProUGUI titleName = null;

        [Inject]
        private RecipeSlot.Factory _slotFactory = null;
        private List<IRecipeSlot> _slots;

        private void Awake()
        {
            _slots = new List<IRecipeSlot>();
        }

        public void FetchRecipes(string label)
        {
            titleName.text = label;
            ClearItems();

            var recipes = RecipeManager.Recipes;
            foreach (var recipe in recipes)
            {
                if (recipe.itemsRequired.Count == 0) continue;
                
                _slots.Add(_slotFactory.Create(recipeContainer));
                _slots[_slots.Count - 1].SetRecipe(recipe).CheckRequirement();
            }

            UpdateSlotsAction();
        }

        public void RefreshRecipeRequirements()
        {
            foreach (var slot in _slots)
                slot.CheckRequirement();
        }

        private void UpdateSlotsAction()
        {
            foreach (var slot in _slots)
                slot.SetSelectAction(() => RecipeSlotAction(slot));
        }

        private void RecipeSlotAction(IRecipeSlot slot)
        {
            if (!cookingList.Full)
            {
                cookingList.AddItem(slot.Item);
                TakeRequiredIngredients(slot.Item);
                RefreshRecipeRequirements();
            }
            else
            {
                //TODO move to game warning notification
                Debug.LogWarning("Max cookable item reached!");
            }
        }

        private void TakeRequiredIngredients(Item item)
        {
            var requiredItems = RecipeManager.GetItemRequirement(item.Code);
            foreach (var requiredItem in requiredItems)
                handler.TakeIngredient(requiredItem);
        }

        private void ClearItems()
        {
            _slots.Clear();
            for (var i = 0; i < recipeContainer.childCount; i++)
                Destroy(recipeContainer.GetChild(i).gameObject);
        }
    }
}