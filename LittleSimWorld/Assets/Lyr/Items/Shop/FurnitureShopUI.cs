namespace Items.Shops.UI {
    using System.Linq;
    using Objects;
	using PlayerStats;
    using UnityEngine;

    public class FurnitureShopUI : GeneralShopUI {
		public override void CompletePurchase() {
			float totalPrice = GetTotalPrice();

			bool success = totalPrice <= Stats.Money;
			if (!success) {
				GameLibOfMethods.CreateFloatingText("Not enough money.", 2);
				AlertNotification();
				return;
			}

			Stats.RemoveMoney(totalPrice);

			foreach (var item in basketItems) {
				var upgradeData = (FurnitureItemData) item.currentItem;
				UpgradesManager.GetValidator(upgradeData.Type).ApplyUpgrade(upgradeData.LinkedPrefab);
				Destroy(item.gameObject);
			}
			basketItems.Clear();

			ResetToDefault();
			currentlyOpenStore.ForceRefresh();
		}

		public override void MoveItemToBasket(ShopItemSlotUI item) {
			// Max stack of 1 for furniture upgrades.
			if (basketItems.Any(x => x.currentItem == item.currentItem)) { return; }
			base.MoveItemToBasket(item);
		}
	}
}