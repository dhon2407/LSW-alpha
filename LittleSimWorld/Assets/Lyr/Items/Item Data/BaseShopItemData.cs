namespace Items {
	using Sirenix.OdinInspector;
	using UnityEngine;

	/// <summary>
	/// Base class for item data that represents items that can be displayed in the shops.
	/// </summary>
	public abstract class BaseShopItemData : SerializedScriptableObject, IShopItem {

#pragma warning disable
		public string _Name;
		[Multiline] public string _Description;
		public float _BasePrice;
		public Sprite _Icon;
#pragma warning restore

		// Base data for every shop item.
		public virtual string Name => _Name;
		public virtual string Description => _Description;
		public virtual float BasePrice => _BasePrice;
		public virtual Sprite Icon => _Icon;

		// Abstract data to be customized for different items.
		public abstract bool ShouldAppearOnShop { get; }
	}


}