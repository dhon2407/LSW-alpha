using UnityEngine;
using System.Collections.Generic;
using InventorySystem;
using PlayerStats;
using TMPro;

namespace Cooking.Recipe
{
    public class CookList : MonoBehaviour
    {
        [SerializeField] private GameObject recipeSlot = null;
        [SerializeField] private Transform itemContainer = null;
        [SerializeField] private CookingHandler handler = null;
        [SerializeField] private RecipeLoader recipeLoader = null;

        [SerializeField] private TextMeshProUGUI buttonText = null;
        [SerializeField] private TextMeshProUGUI deviceName = null;

        public bool Full => _cookingSlots.Count > 0;

        private List<CookSlot> _cookingSlots;
        private void Awake()
        {
            _cookingSlots = new List<CookSlot>();
        }

        public void AddItem(Item slotItem)
        {
            if (_cookingSlots.Exists(cookingSlot => cookingSlot.Item.Code == slotItem.Code))
                _cookingSlots.Find(cookingSlot => cookingSlot.Item.Code == slotItem.Code).AddQuantity();
            else
            {
                var cookSlot = Instantiate(recipeSlot, itemContainer).GetComponent<CookSlot>()?.SetItem(slotItem.Code);

                if (cookSlot != null)
                {
                    _cookingSlots.Add(cookSlot);

                    cookSlot.SetClearAction(() =>
                    {
                        _cookingSlots.Remove(cookSlot);
                        UpdateButtonText();
                    }).SetClickAction(() => ReturnItem(cookSlot.Item));
                }
            }
            
            UpdateButtonText();
        }

        public void ReturnItem(Item item)
        {
            var requiredItems = RecipeManager.GetItemRequirement(item.Code);
            foreach (var requiredItem in requiredItems)
                handler.ReturnIngredient(requiredItem);

            recipeLoader.RefreshRecipeRequirements();
        }

        public void ClearList()
        {
            UpdateButtonText();
            _cookingSlots.Clear();
            for (var i = 0; i < itemContainer.childCount; i++)
                Destroy(itemContainer.GetChild(i).gameObject);
        }

        public void ReturnIngredients()
        {
            if (_cookingSlots.Count > 0)
            {
                foreach (var cookingSlot in _cookingSlots)
                {
                    var requiredItems = RecipeManager.GetItemRequirement(cookingSlot.Item.Code);
                    foreach (var requiredItem in requiredItems)
                        handler.ReturnIngredient(requiredItem, cookingSlot.CurrentItemCount);
                }
            }
            
            ClearList();
        }

        public List<ItemList.ItemInfo> GetRecipesToCook()
        {
            var items = new List<ItemList.ItemInfo>();
            foreach (var cookingSlot in _cookingSlots)
            {
                items.Add(new ItemList.ItemInfo
                {
                    itemCode = cookingSlot.Item.Code,
                    count = cookingSlot.CurrentItemCount,
                });
            }

            return items;
        }

        public void ManualCook()
        {
            if (_cookingSlots.Count == 0) return;

            var itemsToCook = GetRecipesToCook();

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

            CookingEntity.StartAction(itemsToCook, false);
            ClearList();
        }

        public void SetName(string text)
        {
            deviceName.text = text;
        }

        private void UpdateButtonText()
        {
            buttonText.transform.parent.gameObject.SetActive(_cookingSlots.Count > 0);
            buttonText.text = CookingEntity.Text;
        }
        
    }
}