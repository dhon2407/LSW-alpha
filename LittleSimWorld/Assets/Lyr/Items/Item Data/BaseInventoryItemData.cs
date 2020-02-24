namespace Items {
	using Sirenix.OdinInspector;
	using UnityEngine;

	/// <summary>
	/// Base class for item data that represents items that can be placed in inventory.
	/// </summary>
	public abstract class BaseInventoryItemData : SerializedScriptableObject, IShopItem, IInventoryItem {

#pragma warning disable
		[SerializeField] string _Name;
		[SerializeField, Multiline] string _Description;
		[SerializeField] ItemType _Type;
		[SerializeField] float _BasePrice;
		[SerializeField] Sprite _Icon;
#pragma warning restore

		// Base data for every inventory item.
		public string Name => _Name;
		public string Description => _Description;
		public float BasePrice => _BasePrice;
		public Sprite Icon => _Icon;
		public ItemType type => _Type;

		// Abstract data to be customized for different items.
		public abstract bool ShouldAppearOnShop { get; }
		public abstract bool CanUse { get; }
		public abstract bool CanDrop { get; }
		public abstract void Drop();
		public abstract void Use();
	}

}