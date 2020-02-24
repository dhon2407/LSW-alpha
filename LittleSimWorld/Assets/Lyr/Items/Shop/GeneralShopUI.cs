using System;
using LSW.Helpers;

namespace Items.Shops.UI {
    using System.Collections.Generic;
    using GUI_Animations;
	using InventorySystem;
	using PlayerStats;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	public class GeneralShopUI : BaseShopUI {

		[SerializeField] TMPro.TextMeshProUGUI ShopName = null;

		[SerializeField] Transform ShopWindow = null;
		[SerializeField] Transform BasketWindow = null;
		[Space]
		[SerializeField] Button ConfirmButton = null;
		[SerializeField] Button DeclineButton = null;
		[Space]
		[SerializeField] TMPro.TextMeshProUGUI TotalPriceText = null;
		[SerializeField] Color TotalPriceNormalColor = Color.white;
		[Space]
		[Space, Header("Popup Animators")]
		[SerializeField] SimplePopup storePopup = null;
		[SerializeField] SlidingPopup basketPopup = null;

		protected BaseStore currentlyOpenStore;

		private RectTransform _basketRect;

		protected override void Awake() {
			base.Awake();
			ConfirmButton.onClick.AddListener(CompletePurchase);
			DeclineButton.onClick.AddListener(Close);

			_basketRect = basketPopup.GetComponent<RectTransform>();
		}

		protected override void Start()
		{
			base.Start();
			GameLibOfMethods.OnPassOut.AddListener(Close);
		}

		public override void Open<T>(BaseStore<T> shop, bool forceOpen = false) {
			if (!forceOpen && isOpen) {
				Debug.LogError("Shop is already open but Open() was called.");
				return;
			}

			gameObject.SetActive(true);

			if (!isOpen)
				Show(AddShoppingBaskets);
			else 
				AddShoppingBaskets();

			ShopName.text = shop.ShopName;
			currentlyOpenStore = shop;
			shop.isOpen = true;
			isOpen = true;

			void AddShoppingBaskets() {
				foreach (var item in shop.Items) {
					if (!item.ShouldAppearOnShop) { continue; }

					var newObject = CreateShopItemSlot();
					newObject.SetAction(() => MoveItemToBasket(newObject));
					newObject.SetItem(item, item.BasePrice);
				}
			}

		}

		private void Close() => Close(currentlyOpenStore);
		public override void Close(BaseStore shop) {
			if (shop != currentlyOpenStore || !isOpen)
				Debug.LogWarning("Shop's Close() was called from a different shop.");

			if (currentlyOpenStore != null)
			{
				currentlyOpenStore.isOpen = false;
				currentlyOpenStore = null;
			}

			isOpen = false;
			Hide(() =>
			{
				ResetToDefault();
				gameObject.SetActive(false);
			});
		}

		public override void Refresh() {
			foreach (var item in shopItems) { UpdateItemSlot(item); }
			foreach (var item in basketItems) { UpdateItemSlot(item); }
			UpdateTotalPrice();
		}

		public override void ResetToDefault() {

			/// TODO: Optimize with a slot pool.
			foreach (var item in shopItems) { Destroy(item.gameObject); }
			foreach (var item in basketItems) { Destroy(item.gameObject); }

			shopItems.Clear();
			basketItems.Clear();


			ShopName.text = "";
			UpdateTotalPrice();
		}


		public override void MoveItemToBasket(ShopItemSlotUI item) {
			var existingItem = basketItems.Find(x => x.currentItem == item.currentItem);

			// If it exists, add to its quantity
			if (existingItem != null) {
				existingItem.currentQuantity++;
			}
			else {
				var newItem = CreateBasketItemSlot();
				newItem.SetItem(item.currentItem, item.currentPrice, 1);
				newItem.SetAction(() => RemoveItemFromBasket(newItem));
			}

			Refresh();
		}

		public override void RemoveItemFromBasket(BasketItemSlotUI item) {
			if (!basketItems.Contains(item)) { Debug.LogError("Item was not found on the basket."); }

			// If it exists more than once, substract from its quantity
			if (item.currentQuantity > 1) {
				item.currentQuantity--;
			}
			else {
				basketItems.Remove(item);
				Destroy(item.gameObject);
			}

			Refresh();
		}

		public override void CompletePurchase() {
			float totalPrice = GetTotalPrice();

			if (totalPrice > Stats.Money) {
				GameLibOfMethods.CreateFloatingText("Not enough money", 2);
				AlertNotification();
				return;
			}

			var itemInfos = new List<ItemList.ItemInfo>(basketItems.Count);

			foreach (var item in basketItems) {
				var itemCode = ((ItemData) item.currentItem).code;
				var info = new ItemList.ItemInfo() {
					itemCode = itemCode,
					count = item.currentQuantity
				};
				itemInfos.Add(info);
			}
			bool success = Inventory.PlaceOnBag(itemInfos);
			if (!success) {
				GameLibOfMethods.CreateFloatingText("Not enough space in inventory", 2);
				AlertNotification();
				return;
			}

			Stats.RemoveMoney(totalPrice);
			foreach (var item in basketItems) { Destroy(item.gameObject); }
			basketItems.Clear();
			Refresh();
		}

		#region Utilities

		private void Show(UnityAction action = null) => storePopup.Show(() => basketPopup.Show(action));
		private void Hide(UnityAction action = null) => basketPopup.ForceHide(() => storePopup.Hide(action));

		private ShopItemSlotUI CreateShopItemSlot() {
			var item = Instantiate(ShopUISlot);
			item.transform.SetParent(ShopWindow);
			item.transform.localScale = Vector3.one;
			shopItems.Add(item);
			return item;
		}

		private BasketItemSlotUI CreateBasketItemSlot() {
			var item = Instantiate(BasketUISlot);
			item.transform.SetParent(BasketWindow);
			item.transform.localScale = Vector3.one;
			basketItems.Add(item);
			return item;
		}

		protected float GetDiscountedPrice(float basePrice) {
			/// TODO: Apply discounts etc here
			return basePrice;
		}

		protected float GetTotalPrice() {
			float totalPrice = 0;
			foreach (var bItem in basketItems) { totalPrice += bItem.currentPrice * bItem.currentQuantity; }
			return totalPrice;
		}

		protected void UpdateItemSlot(BasketItemSlotUI basketItem) => basketItem.SetItem(basketItem.currentItem, basketItem.currentPrice, basketItem.currentQuantity);
		protected void UpdateItemSlot(ShopItemSlotUI shopItem) => shopItem.SetItem(shopItem.currentItem, shopItem.currentPrice);
		protected void UpdateTotalPrice() {
			float totalPrice = GetTotalPrice();
			TotalPriceText.text = "£ " + totalPrice.ToString("0.00");
			TotalPriceText.color = (totalPrice <= Stats.Money) ? TotalPriceNormalColor : Color.red;
		}
		#endregion

		protected void AlertNotification()
		{
			_basketRect.Shake(0.2f, 5f);
		}
	}
}