namespace Objects.Utilities {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Sirenix.OdinInspector;
	using System.Linq;

	public enum UpgradeType { Bed, Chair, Desk, Fridge, MixingBar, Computer, Shower, Sink, Stove, Toilet, Wardrobe }

	[ExecuteInEditMode]
	public class UpgradeValidator : MonoBehaviour {

		public UpgradeType ValidatorType;

		[Space]
		[ValueDropdown("GetPrefabValues", AppendNextDrawer = true)]
		[OnValueChanged("UpdateSelected")]
		public GameObject CurrentObject;

		public Vector3 OriginalPosition;

		[ListDrawerSettings(HideAddButton = true)]
		public List<UpgradeObjectSettings> ObjectSettings = new List<UpgradeObjectSettings>();

		/// <summary>
		/// Apply the upgrade to the object.
		/// <para>Switches the prefab, including the functionality and custom position.</para>
		/// </summary>
		/// <param name="prefab">The prefab of the object you wish to apply.</param>
		/// <param name="CallFromEditor">Was the call made from editor? (used for preview only)</param>
		public void ApplyUpgrade(GameObject prefab, bool CallFromEditor = false) {
#if UNITY_EDITOR
			if (CallFromEditor) {
				EditModeSelect(ObjectSettings.Find(x => x.Prefab == prefab));
				return;
			}
#endif
			if (transform.childCount > 1) { Debug.LogError("Wrong amount of children in Transform."); return; }
			else if (transform.childCount == 1) { DestroyImmediate(transform.GetChild(0).gameObject); }

			var obj = ObjectSettings.Find(x => x.Prefab == prefab);
			if (obj == null) { throw new UnityException("Object is not part of the Upgradable's list."); }

			CurrentObject = obj.Prefab;

			//Debug.Log("Updating Selection");
			var newObj = Instantiate(obj.Prefab, transform);
			newObj.name = newObj.name.Replace("(Clone)", "");
			transform.localScale = obj.LocalScale;
			transform.position = OriginalPosition + obj.LocalPosition;
		}

		public bool CanApplyUpgrade(GameObject prefab) {
			var currentPrefab = ObjectSettings.Find(x => x.Prefab == CurrentObject);
			var targetPrefab = ObjectSettings.Find(x => x.Prefab == prefab);
			if (currentPrefab == null || targetPrefab == null) { Debug.LogError("Cannot find target Upgrade"); }

			return ObjectSettings.IndexOf(currentPrefab) == ObjectSettings.IndexOf(targetPrefab) - 1;
		}

		[System.Serializable]
		public class UpgradeObjectSettings {


			public GameObject Prefab;

			[Space, Header("Transform Settings")]
			public Vector3 LocalPosition;
			public Vector3 LocalScale;

			public UpgradeObjectSettings(GameObject prefab) {
				Prefab = prefab;
				LocalPosition = Vector3.zero;
				LocalScale = Vector3.one;
			}

			public override string ToString() => Prefab.name;
		}


#if UNITY_EDITOR
		#region Editor Functionality

		/// <summary>Update selection for instant preview.</summary>
		void EditModeSelect(UpgradeObjectSettings obj) {
			if (transform.childCount > 1) { Debug.LogError("Wrong amount of children in Transform."); return; }
			else if (transform.childCount == 1) { DestroyImmediate(transform.GetChild(0).gameObject); }

			if (obj == null) { throw new UnityException("Object you want to select is null"); }
			// Reset TransformSettingsType so it won't fuck up stuff.
			if (type == TransformSettingsType.UpdateBaseTransform) { type = 0; }

			Debug.Log("Updating Selection");
			CurrentObject = obj.Prefab;
			var newObj = UnityEditor.PrefabUtility.InstantiatePrefab(obj.Prefab, transform);
			newObj.name = newObj.name.Replace("(Clone)", "");

			transform.localScale = obj.LocalScale;
			transform.position = OriginalPosition + obj.LocalPosition;
		}


		enum TransformSettingsType { None, UpdateBaseTransform, UpdateSelectedObjectTransform }

		[Header("Editor Functionality"), ShowInInspector]
		bool ShowEditorSettings;

		[DetailedInfoBox("Warning: Real-time transform update for the editor! (Click to expand)",
		"Real-time transform update for the editor!\n\n" +
		" - None: Does nothing.\n" +
		" - UpdateBaseTransform : Updates the OriginalPosition for the base Object.\n" +
		" - UpdateSelectedObjectTransform: Updates the LocalPosition for the Selected Object."
		, InfoMessageType = InfoMessageType.Info)]
		[ShowInInspector, EnumToggleButtons, HideLabel] TransformSettingsType type = 0;

		[InfoBox("Add new upgrades to the list from here.\nFill the list and click 'Add Settings for New Prefabs'.")]
		[ShowIf("ShowEditorSettings")] public List<GameObject> prefabsToAddToTheList = new List<GameObject>();

		/// Button that generates default settings for the prefabs in <see cref="prefabsToAddToTheList"/>.
		[ShowIf("ShowEditorSettings"), Button]
		void AddSettingsForNewPrefabs() {
			foreach (var prefab in prefabsToAddToTheList) {
				if (ObjectSettings.Any(x => x.Prefab == prefab)) {
					Debug.LogWarning($"{prefab.name} is already contained in the list. Skipping...");
					continue;
				}
				var newSettings = new UpgradeObjectSettings(prefab);
				ObjectSettings.Add(newSettings);
			}
			if (prefabsToAddToTheList.Count != 0) {
				OriginalPosition = transform.position;
				prefabsToAddToTheList.Clear();
			}
		}


		/// Used in <see cref="CurrentObject"/>'s drawer settings.
		void UpdateSelected() => EditModeSelect(ObjectSettings.Find(x => x.Prefab == CurrentObject));

		void Awake() { }
		void OnEnable() { type = 0; }
		void OnDisable() { type = 0; }
		void Update() { if (!Application.isPlaying) { OnValidate(); } }
		void OnValidate() => ValidateTransformDifferences();
		void ValidateTransformDifferences() {
			if (UnityEditor.Selection.activeGameObject != gameObject) { return; }

			if (type == TransformSettingsType.None) { return; }
			else if (type == TransformSettingsType.UpdateBaseTransform) {
				OriginalPosition = transform.position;
			}
			else if (type == TransformSettingsType.UpdateSelectedObjectTransform) {
				if (ObjectSettings == null) { return; }
				var settings = ObjectSettings.Find(x => x.Prefab == CurrentObject);
				if (settings == null) {
					Debug.LogError("Item not found in settings. Update will not be saved.");
					return;
				}

				if (settings.LocalScale != transform.localScale) {
					Debug.Log("Updating local scale for current object.");
					Debug.Log("Previous value was: " + settings.LocalScale);
					settings.LocalScale = transform.localScale;
				}

				if (settings.LocalPosition != transform.position - OriginalPosition) {
					Debug.Log("Updating position for current object.");
					Debug.Log("Previous value was: " + settings.LocalPosition);
					settings.LocalPosition = transform.position - OriginalPosition;
				}
			}
		}
		/// Used in <see cref="CurrentObject"/>'s drawer settings.
		ValueDropdownList<GameObject> GetPrefabValues() {
			var list = new ValueDropdownList<GameObject>();
			foreach (var prefabSettings in ObjectSettings) {
				list.Add(prefabSettings.Prefab.name, prefabSettings.Prefab);
			}
			return list;
		}

		#endregion
#endif

	}
}