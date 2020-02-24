using Items;
using UnityEngine;

namespace InventorySystem {

	///<summary>
	///Existing Item Data for Shops and Inventory. 
	///<para>TODO: Make it an <see cref="IInventoryItem"/>.</para>
	///</summary>
	[CreateAssetMenu(fileName = "Item", menuName = "Items/Basic")]
	public class ItemData : ScriptableObject, IShopItem {
		public ItemCode code;
		public ItemKind kind;
		public new string name;
		public Sprite icon;
		public string Description;
        public ItemType ItemType;
		[Space]
		public bool droppable;
		[Space]
		public bool isStackable;
		public int maxStack;
		[Space]
		public float price;

		public virtual ItemState State => ItemState.Passive;

		public string Name => name;
		public float BasePrice => price;
		public Sprite Icon => icon;
		public bool ShouldAppearOnShop => true;
		string IShopItem.Description => Description;

		public enum ItemState {
			Active,
			Passive,
		}

		public enum ItemKind {
			None,
			Ingredient,
			Furniture,
		}
	}
}