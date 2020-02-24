using System;

namespace Items.Shops {
	using System.Collections.Generic;
	using Items.Shops.UI;
	using UnityEngine;

	/// <summary>
	/// UI Base for shops.
	/// </summary>
	public abstract class BaseShopUI : MonoBehaviour {

		/// <summary>
		/// Is the shop currently open?
		/// </summary>
		public static bool isOpen { get; set; }

		/// <summary>List of available items.</summary>
		protected List<ShopItemSlotUI> shopItems = new List<ShopItemSlotUI>();
		/// <summary> List of items currently in the basket.</summary>
		protected List<BasketItemSlotUI> basketItems = new List<BasketItemSlotUI>();

		/// <summary>Open the UI for the shop and initialize.</summary>
		public abstract void Open<T>(BaseStore<T> shop, bool forceOpen = false) where T : class, IShopItem;

		/// <summary> Close the UI for the shop and clear cache.</summary>
		public abstract void Close(BaseStore shop = null);



		/// <summary> Refresh the item list on the shop UI.</summary>
		public abstract void Refresh();

		/// <summary> Revert the shop UI to its original state.</summary>
		public abstract void ResetToDefault();



		/// <summary>Moves the specified item to the basket.</summary>
		public abstract void MoveItemToBasket(ShopItemSlotUI item);

		/// <summary>Removes the item from the basket.</summary>
		public abstract void RemoveItemFromBasket(BasketItemSlotUI item);



		/// <summary> Finalizes the purchase.</summary>
		public abstract void CompletePurchase();


		/// <summary> The Shop UI Slot</summary>
		protected static ShopItemSlotUI ShopUISlot;
		/// <summary> The Basket UI Slot</summary>
		protected static BasketItemSlotUI BasketUISlot;

		protected virtual void Awake() {
			ShopUISlot = Resources.Load<ShopItemSlotUI>("Inventory/Shops/Item UI Slot");
			BasketUISlot = Resources.Load<BasketItemSlotUI>("Inventory/Shops/Basket UI Slot");
		}

		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
	}

}