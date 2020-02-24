namespace Items {

	/// <summary>
	/// Enum that represents the type of the inventory item.
	/// </summary>
	public enum ItemType { Food, Drink, Ingredient, Quest, Junk, Other }

	/// <summary>
	/// Interface for items that can be placed in the inventory.
	/// </summary>
	public interface IInventoryItem {
		/// <summary>
		/// The name of the item.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The description of the item.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// The type of the item.
		/// </summary>
		ItemType type { get; }

		/// <summary>
		/// Can the item be used?
		/// </summary>
		bool CanUse { get; }

		/// <summary>
		/// Can the item be dropped?
		/// </summary>
		bool CanDrop { get; }

		/// <summary>
		/// Use the item from the inventory.
		/// </summary>
		void Use();

		/// <summary>
		/// Drop the item from the inventory.
		/// </summary>
		void Drop();
	}
}