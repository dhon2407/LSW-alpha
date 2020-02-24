using UnityEngine;

namespace Items {

	[CreateAssetMenu(menuName = "Items/Inventory Item")]
	public class InventoryItemData : BaseInventoryItemData {
		public override bool ShouldAppearOnShop => true;
		public override bool CanUse => true;
		public override bool CanDrop => true;

		public override void Drop() => throw new System.NotImplementedException();
		public override void Use() => throw new System.NotImplementedException();
	}


}