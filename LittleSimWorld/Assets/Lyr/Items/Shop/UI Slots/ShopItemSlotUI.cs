namespace Items.Shops.UI {
	using UnityEngine;

    public class ShopItemSlotUI : BaseItemSlotUI {
		[SerializeField] protected TMPro.TextMeshProUGUI price = null;

		[System.NonSerialized] public float currentPrice;

		public void SetItem(IShopItem item, float price) {
			currentItem = item;
			currentPrice = price;

			this.price.text = "£ " + price.ToString("0.00");
			name.text = currentItem.Name;
			icon.sprite = currentItem.Icon;
		}
	}
}