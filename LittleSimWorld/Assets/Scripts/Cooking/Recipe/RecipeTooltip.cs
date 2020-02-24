using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using LSW.Tooltip;
using PlayerStats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    [AddComponentMenu(menuName:"Tooltip/Recipe Tooltip")]
    public class RecipeTooltip : Tooltip<RecipeTooltip.Data>
    {
        [Header("Labels")]
        [SerializeField] private TextMeshProUGUI recipeName = null;
        [SerializeField] private TextMeshProUGUI description = null;
        [SerializeField] private TextMeshProUGUI quickPrice = null;
        
        [Space]
        [SerializeField] private Transform ingredientsList = null;

        private bool _sameItem;
        private VerticalLayoutGroup _layoutGroup;

        public override void SetData(Data data)
        {
            SetupData(data.item,
                RecipeManager.GetItemRequirement(data.item.Data.code),
                data.refreshItem,
                data.isLocked,
                data.isSlotLocked,
                data.quickCookLocked,
                data.price);
        }

        public override void Show()
        {
            if (!_sameItem)
                StartCoroutine(DelayOpen());
            else
                base.Show();
        }

        public override void Hide()
        {
            StopAllCoroutines();
            base.Hide();
        }

        private void Start()
        {
            _layoutGroup = GetComponent<VerticalLayoutGroup>();
        }

        private IEnumerator DelayOpen()
        {
            _layoutGroup.enabled = false;
            yield return null;
            _layoutGroup.enabled = true;
            yield return null;

            base.Show();
        }

        private void SetupData(Item recipe, List<Item> ingredients,
            bool sameItem, bool itemLocked, bool slotLocked, bool quickCookLocked,
            float price = 0)
        {
            ClearIngredients();

            
            _sameItem = sameItem;

            recipeName.text = (itemLocked || slotLocked || quickCookLocked) ? "Unknown" : recipe.Data.name;
            quickPrice.text = string.Empty;

            if (itemLocked)
                description.text = "Not enough ingredients.";
            else if (slotLocked)
                description.text = "Raise your cooking level to unlock this.";
            else
                description.text = recipe.Data.Description;
            
            if (quickCookLocked)
                description.text = "Unlock this recipe by cooking it manually first.";

            if (price > 0)
            {
                quickPrice.text = $"£ {price}";
                quickPrice.color = (price <= Stats.Money) ? Color.black : Color.red;
            }

            for (var i = 0; i < ingredients.Count; i++)
            {
                var ingredientObject = ingredientsList.GetChild(i).gameObject;
                ingredientObject.SetActive(true);
                ingredientObject.GetComponent<IngredientTooltip>().SetItem(ingredients[i]);
            }
            
            UpdateIngredientsView();
        }

        private void UpdateIngredientsView()
        {
            var previousIngredientsCount = new Dictionary<ItemCode, int>();
            
            for (int i = 0; i < ingredientsList.childCount; i++)
            {
                var ingTooltip = ingredientsList.GetChild(i).GetComponent<IngredientTooltip>();
                if (CookingHandler.AvailableIngredients.Exists(ing =>
                    ing.itemCode == ingTooltip.ItemCode))
                {
                    var availableIngredient =
                        CookingHandler.AvailableIngredients.Find(ing => ing.itemCode == ingTooltip.ItemCode);

                    if (previousIngredientsCount.ContainsKey(ingTooltip.ItemCode))
                    {
                        ingTooltip.GreyOut((availableIngredient.count - previousIngredientsCount[ingTooltip.ItemCode]) <= 0);

                        previousIngredientsCount[ingTooltip.ItemCode]++;
                    }
                    else
                    {
                        ingTooltip.GreyOut(availableIngredient.count == 0);

                        previousIngredientsCount[ingTooltip.ItemCode] = 1;
                    }
                }
                else
                {
                    ingTooltip.GreyOut(true);
                }
            }
        }
        
        private void ClearIngredients()
        {
            for (var i = 0; i < ingredientsList.childCount; i++)
                ingredientsList.GetChild(i).gameObject.SetActive(false);
        }

        public struct Data
        {
            public Item item;
            public bool isLocked;
            public bool isSlotLocked;
            public bool refreshItem;
            public bool quickCookLocked;
            public float price;
        }
    }
}