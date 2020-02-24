namespace Items.Shops.UI {
	using LSW.Tooltip;
	using UnityEngine;

    public class BasketItemSlotUI : BaseItemSlotUI {
		[SerializeField] TMPro.TextMeshProUGUI price = null;
		[SerializeField] TMPro.TextMeshProUGUI quantity = null;

		[System.NonSerialized]public float currentPrice;
		[System.NonSerialized]public int currentQuantity;

		public void SetItem(IShopItem item, float price, int quantity) {
			currentItem = item;
			currentPrice = price;
			currentQuantity = quantity;

			if (currentQuantity == 0) { Debug.LogWarning("Quantity is 0 but SetItem() was called."); }

			this.price.text = "£ " + price.ToString("0.00");
			this.quantity.transform.parent.gameObject.SetActive(currentQuantity > 1);
			this.quantity.text = quantity.ToString();
			name.text = currentItem.Name;
			icon.sprite = currentItem.Icon;
		}
	}
}