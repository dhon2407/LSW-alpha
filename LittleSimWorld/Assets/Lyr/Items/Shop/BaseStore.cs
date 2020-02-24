using UI;

namespace Items.Shops {
	using System;
	using System.Collections.Generic;
	using InventorySystem;
	using Objects;
	using Sirenix.OdinInspector;
	using UnityEngine;

	/// <summary>
	/// Non-generic abstract class to be used as a reference in shops.
	/// </summary>
	public abstract class BaseStore : InteractableObject {
		[NonSerialized] public bool isOpen;
		public abstract void ForceRefresh();
	}

	/// <summary>
	/// Base for interactable shops.
	/// </summary>
	/// <typeparam name="T">The type of the items it sells.</typeparam>
	public abstract class BaseStore<T> : BaseStore where T : class, IShopItem {

		[Header("Store Settings")]
		[SceneObjectsOnly]
		public BaseShopUI shopUI;
		public string ShopName;

		public List<T> Items;

		Collider2D col => _col ? _col : (_col = GetComponent<Collider2D>());
		Collider2D _col;

		public override void Interact() {
			if (!isOpen) { shopUI.Open(this); }
			else { shopUI.Close(this); }
		}
		public override void ForceRefresh() => shopUI.Open(this, true);
		protected virtual void Update() {
			if (!isOpen) { return; }
			Vector2 playerPos = Player.position;
			var distanceFromPlayer = (playerPos - col.ClosestPoint(playerPos)).sqrMagnitude;
			if (distanceFromPlayer > 0.5f) { shopUI.Close(this); }
		}

		private void Start()
		{
			GamePauseHandler.SubscribeCloseEvent(EscCloseEvent);
		}

		private bool EscCloseEvent()
		{
			if (!isOpen) return false;

			shopUI.Close(this);
			return true;
		}

		/*
		#region Editor
#if UNITY_EDITOR
		[ShowIf("ShowAddedOnly"), ShowInInspector, PropertyOrder(61),DoNotDrawAsReference]
		List<T> _AddedItems {
			get => Items;
			set { }
		}

		[PropertyOrder(59), ShowInInspector] bool ShowAddedOnly = true;
		[ValueDropdown("GetFilteredTypeList"),HideIf("ShowAddedOnly")]
		[SerializeField, PropertyOrder(60)] 
		List<Type> itemTypesOnShop = new List<Type>();
		bool FilterAssets(T item) => itemTypesOnShop.Contains(item.GetType());

		ValueDropdownList<Type> GetFilteredTypeList() {
			var list = new ValueDropdownList<Type>();

			var types = typeof(T).Assembly.GetTypes();
			foreach (var type in types) {
				if (type.IsAbstract) { continue; }
				if (type.IsGenericTypeDefinition) { continue; }
				if (type.ContainsGenericParameters) { continue; }
				if (!typeof(T).IsAssignableFrom(type)) { continue; }
				if (this.itemTypesOnShop.Contains(type)) { continue; }
				list.Add(type.Name, type);
			}

			return list;
		}

		ValueDropdownList<ScriptableObject> GetAllShopItems(){
			var list = new ValueDropdownList<ScriptableObject>();
			var assetGUIDs = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			foreach (var guid in assetGUIDs) {
				var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
				if (Items.Contains(asset as T)) { continue; }
				list.Add((asset as T).Name, asset);
			}

			return list;
		}

#endif
		#endregion
		*/
	}

}