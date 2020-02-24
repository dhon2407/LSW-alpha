#if UNITY_EDITOR
namespace Utilities.Creators {
	using System.Collections.Generic;
    using System.Linq;
    using Items;
	using Items.Shops;
	using Objects.Utilities;
	using Sirenix.OdinInspector;
	using UnityEditor;
	using UnityEngine;

	public class FurnitureItemDataCreator : ScriptableObjectCreator<FurnitureItemData> {

		[ShowInInspector]
		UpgradeType SetType { get; set; } = (UpgradeType) (-1);

		protected override string SavePath => "Assets/ScriptableObjects/Resources/Furniture Upgrades/" + SetType.ToString();

		protected override void AdditionalInitialization(FurnitureItemData obj) { }
		protected override bool AdditionalValidation() {
			if ((int)SetType == (-1)) {
				Debug.Log("Select a Set Type from above");
				return false;
			}
			return true;
		}

		protected override void AdditionalReset() { }// => SetType = CharacterPart.None;



		[Header("Mass Import")]
		[PropertyOrder(999)] public string Prefix;
		[PropertyOrder(1001)] public UpgradeType upgradeType = (UpgradeType) (-1);
		[PropertyOrder(1002), AssetsOnly] public List<GameObject> ObjectPrefabs = new List<GameObject>();

		[Button]
		[PropertyOrder(1002)]
		void MassSpawn() {
			for (int i = 0; i < 7; i++) {
				item._Name = GetPrefix(i) + " " + upgradeType.ToString();
				item._BasePrice = GetPrice(i);
				item.Type = upgradeType;
				item.LinkedPrefab = ObjectPrefabs[i];

				Name = item._Name;
				SetType = upgradeType;

				CreateAsset();
			}


			ObjectPrefabs.Clear();
		}

		string GetPrefix(int i) {
			if (i == 0) { return "Broken"; }
			if (i == 1) { return "Old"; }
			if (i == 2) { return "Budget"; }
			if (i == 3) { return "Regular"; }
			if (i == 4) { return "Nice"; }
			if (i == 5) { return "Special"; }
			if (i == 6) { return "Perfect"; }

			Debug.Log("WRONG INDEX");
			return "ASDF";
		}

		int GetPrice(int i) {
			if (i == 0) { return 0; }
			if (i == 1) { return 50; }
			if (i == 2) { return 200; }
			if (i == 3) { return 500; }
			if (i == 4) { return 1000; }
			if (i == 5) { return 2500; }
			if (i == 6) { return 5000; }

			Debug.Log("WRONG INDEX");
			return 0;
		}
		public FurnitureStore Store;

		[Button]
		void CacheIcons() {
			var guids = AssetDatabase.FindAssets("t:FurnitureItemData");
			foreach (var guid in guids) {
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<FurnitureItemData>(path);
				Store.Items.Add(asset);
				//Store.Items.OrderBy(x => ((int)x.Type * 1000000f)+ (x.BasePrice * 10f));
				Store.Items = Store.Items.OrderBy(x => x._BasePrice).OrderBy(x => (int) x.Type).ToList();
			}
		}
	}
}
#endif
