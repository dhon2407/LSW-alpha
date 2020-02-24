using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using LSW.Helpers;
using LSW.Tooltip;
using PlayerStats;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

using static Cooking.Recipe.CookingHandler;
using static Cooking.Recipe.SlotVisibility;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Cooking.Recipe
{
    public class QuickRecipeSlot : TooltipArea<RecipeTooltip.Data>, IRecipeSlot
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Button button;

        [Header("Icon color tint")]
        [SerializeField] protected Color available = Color.white;
        [SerializeField] protected Color locked = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);
        [SerializeField] protected Color hidden = new Color(10f / 255f, 10f / 255f, 10f / 255f, 70f / 255f);

        [Space]
        [Header("Shake Behaviour")]
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private float intensity = 0.02f;

        protected Item currentItem;
        protected SlotVisibility visibility;
        private bool _refreshItem;
        private bool _mouseOver;
        private float _price;

        public Item Item => currentItem;
        
        public IRecipeSlot SetRecipe(NewRecipe recipe)
        {
            currentItem = Inventory.CreateItem(recipe.RecipeOutcome);
            icon.sprite = currentItem.Icon;

            return this;
        }

        public void SetSelectAction(UnityAction action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                if (visibility == Available)
                    action.Invoke();
                else
                    CantCookThis();
            });
        }

        protected void Reset()
        {
            button = GetComponent<Button>();
        }

        public void CheckRequirement()
        {
            float multiplier = 1;
            _price = GetTotalCost() * multiplier;

            if (CookedRecipes.Contains(currentItem.Code) || currentItem.Code == ItemCode.Jelly)
                SetVisibility(_price <= Stats.Money ? Available : Locked);
            else
                SetVisibility(SlotLock);


            SetSelectAction(Cook);
        }

        private float GetTotalCost()
        {
            var ingredients = RecipeManager.GetItemRequirement(currentItem.Code);

            return ingredients.Sum(ingredient => ingredient.Data.price);
        }

        private void Start()
        {
            onMouseEnter.AddListener(() => _mouseOver = true); 
            onMouseExit.AddListener(()=> _mouseOver = false);
        }

        private void SetVisibility(SlotVisibility value)
        {
            visibility = value;
            button.interactable = (visibility == Available ||
                                   visibility == Locked);

            if (button.interactable)
                RefreshTooltip();

            icon.color = GetIconColor();
        }

        private Color GetIconColor()
        {
            switch (visibility)
            {
                case Available: return available;
                case Locked: return locked;
                default: return hidden;
            }
        }

        private void RefreshTooltip()
        {
            if (!_mouseOver) return;
            
            _refreshItem = true;
            ToolTip.SetData(TooltipData);
            ToolTip.Show();
            _refreshItem = false;
        }

        private void CantCookThis()
        {
            button.Shake(duration, intensity);
        }
        
        private void Cook()
        {
            var itemsToCook = GetRecipeToCook();

            if (!Inventory.CanFitOnBag(itemsToCook))
            {
                GameLibOfMethods.CreateFloatingText("Not enough space in inventory.", 2f);
                return;
            }
            
            if (PlayerStatsManager.statWarning.ContainsValue(true))
            {
                PlayerChatLog.Instance.AddChatMessege("I'm not feeling well to cook.");
                return;
            }
            
            Stats.RemoveMoney(_price);
            CookingEntity.StartAction(itemsToCook, true);
        }

        private List<ItemList.ItemInfo> GetRecipeToCook()
        {
            return new List<ItemList.ItemInfo>
            {
                new ItemList.ItemInfo {itemCode = currentItem.Code, count = 1,}
            };
        }
        
        private RecipeTooltip.Data DisplayData()
        {
            var data = new RecipeTooltip.Data
            {
                isLocked = false,
                isSlotLocked = false,
                quickCookLocked = (visibility == SlotLock),
                refreshItem = _refreshItem,
                item = currentItem,
                price = _price,
            };

            return data;
        }
        
        protected override RecipeTooltip.Data TooltipData => DisplayData();

        public class Factory : PlaceholderFactory<QuickRecipeSlot> { }
    }
}