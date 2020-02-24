using System;
using InventorySystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cooking.Recipe
{
    public class CookSlot : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI qty = null;
        [SerializeField] protected Image icon;
        [SerializeField] protected Button button;

        public Item Item => currentItem;
        
        private Item currentItem;
        private int currentQty = 1;
        private UnityAction onDestroyAction;
        private UnityAction onClickAction;

        public int CurrentItemCount => currentQty;

        protected void Start()
        {
            UpdateDisplay();
            button.onClick.AddListener(ReduceQuantity);
            button.onClick.AddListener(onClickAction);
        }

        private void ReduceQuantity()
        {
            ReduceQuantity(1);
        }

        public void AddQuantity(int qty = 1)
        {
            currentQty = Mathf.Clamp(currentQty + qty, 0, int.MaxValue);
            UpdateDisplay();
        }

        public void ReduceQuantity(int qty = 1)
        {
            AddQuantity(-qty);
        }

        public CookSlot SetItem(ItemCode recipeRecipeOutcome)
        {
            currentItem = Inventory.CreateItem(recipeRecipeOutcome);
            icon.sprite = currentItem.Icon;

            return this;
        }

        public CookSlot SetClearAction(UnityAction action)
        {
            onDestroyAction = action;
            return this;
        }

        public CookSlot SetClickAction(UnityAction action)
        {
            onClickAction = action;
            return this;
        }

        private void UpdateDisplay()
        { 
            qty.transform.parent.gameObject.SetActive(true);

            if (currentQty > 1)
                qty.text = currentQty.ToString("0");
            else if (currentQty == 1)
                qty.transform.parent.gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            onDestroyAction?.Invoke();
        }
    }
}