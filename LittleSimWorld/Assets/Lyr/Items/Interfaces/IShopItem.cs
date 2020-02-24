namespace Items {
	using UnityEngine;

	/// <summary>
	/// Interface for items that can be displayed on the shop.
	/// </summary>
	public interface IShopItem {
		/// <summary>
		/// The name of the item.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The description of the item.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// The base price for the item.
		/// </summary>
		float BasePrice { get; }

		/// <summary>
		/// The icon for the Shop UI which will be used for the item.
		/// </summary>
		Sprite Icon { get; }

		/// <summary>
		/// Should the item appear on the shop?
		/// </summary>
		bool ShouldAppearOnShop { get; }
	}

}