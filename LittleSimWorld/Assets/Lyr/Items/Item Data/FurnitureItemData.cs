namespace Items {
	using Objects;
	using UnityEngine;
	using UpgradeType = Objects.Utilities.UpgradeType;

	[CreateAssetMenu(menuName = "Items/Furniture")]
	public class FurnitureItemData : BaseShopItemData {
		public UpgradeType Type;
		public GameObject LinkedPrefab;

		public override bool ShouldAppearOnShop {
			get {
				var validator = UpgradesManager.GetValidator(Type);
				return validator.CanApplyUpgrade(LinkedPrefab);
			}
		}

	}


}