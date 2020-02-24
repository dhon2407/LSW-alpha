using InventorySystem;
using LSW.Helpers;
using LSW.Tooltip;
using PlayerStats;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

using static Cooking.Recipe.CookingHandler;
using static Cooking.Recipe.SlotVisibility;

namespace Cooking.Recipe
{
    public class RecipeSlot : TooltipArea<RecipeTooltip.Data>, IRecipeSlot
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
        protected NewRecipe currentRecipe;
        protected SlotVisibility visibility;
        private bool _refreshItem;
        private bool _mouseOver;

        public Item Item => currentItem;
        
        public IRecipeSlot SetRecipe(NewRecipe recipe)
        {
            currentRecipe = recipe;
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
            var requiredItems = RecipeManager.GetItemRequirement(currentItem.Code);
            var slotRequirements = requiredItems.Count;

            if (CookingEntity.SlotRequiredLevel(slotRequirements) > CookingEntity.NeededSkillLevel)
            {
                SetVisibility(SlotLock);
                return;
            }

            if (RecipeManager.HaveEnoughIngredients(currentRecipe, AvailableIngredients))
            {
                SetVisibility(Available);

                if (!SeenRecipes.Contains(currentItem.Code))
                    SeenRecipes.Add(currentItem.Code);
            }
            else
            {
                SetVisibility(CookedRecipes.Contains(currentItem.Code) ||
                              SeenRecipes.Contains(currentItem.Code) ?
                    Locked : Hidden);
            }
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
        
        private RecipeTooltip.Data DisplayData()
        {
            RecipeTooltip.Data data = new RecipeTooltip.Data
            {
                isLocked = (visibility == Hidden),
                isSlotLocked = (visibility == SlotLock),
                refreshItem = _refreshItem,
                item = currentItem,
                quickCookLocked = false,
                price = 0,
            };

            return data;
        }
        
        protected override RecipeTooltip.Data TooltipData => DisplayData();

        public class Factory : PlaceholderFactory<RecipeSlot> { }
    }
}
