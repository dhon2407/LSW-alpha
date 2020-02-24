#if UNITY_EDITOR
namespace Objects.Editor {
	using System.Collections.Generic;
	using System.Linq;
	using Objects.Utilities;
	using PlayerStats;
	using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities.Editor;
	using UnityEditor;
	using UnityEngine;

	public class UpgradeValidatorEditor : OdinEditorWindow {

		List<UpgradeValidator> validators;
		Vector2 scrollPosition;

		public static UpgradeValidatorEditor instance;
		[MenuItem("Tools/Upgrade Validator Window")]
		public static void Init() => instance = GetWindow<UpgradeValidatorEditor>("Upgrade Validator Editor Window");

		protected override void OnEnable() {
			base.OnEnable();
			InitializeValidators();
		}

		void InitializeValidators() => validators = GameObject.FindObjectsOfType<UpgradeValidator>().ToList();

		[Button]
		void SetPlayerStatsToHalf() {
			if (!EditorApplication.isPlaying) {
				Debug.LogError("Only valid if playing.");
				return;
			}

			var statManager = FindObjectOfType<PlayerStatsManager>();
			var statuses = statManager.PlayerStatus;
			foreach (var statKey in statuses.Keys) {
				statuses[statKey].Set(50);
			}
		}
		protected override void OnGUI() {
			if (validators == null || validators.Any(x => x == null)) { InitializeValidators(); }

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false);

			EditorGUILayout.BeginVertical();
			foreach (var validator in validators) {
				DrawValidator(validator);
			}
			EditorGUILayout.EndVertical();
			base.OnGUI();
			EditorGUILayout.EndScrollView();
		}

		void DrawValidator(UpgradeValidator validator) {
			int i = 0;

			EditorGUILayout.LabelField(validator.name, EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(40 * validator.ObjectSettings.Count));

			// Draw Left Button
			if (DrawSwitchButton(EditorIcons.ArrowLeft)) { SwitchOption(validator, -1); }

			foreach (var item in validator.ObjectSettings) {
				EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("    " + i++, GUILayout.Width(35), GUILayout.Height(10));
				var rect = EditorGUILayout.GetControlRect(GUILayout.Height(30), GUILayout.Width(35));
				if (item.Prefab == validator.CurrentObject) { DrawBoxForSelectedObject(rect); }
				SirenixEditorFields.PreviewObjectField<GameObject>(rect, item.Prefab, true, false, false, false);

				if (DidClickOccur(rect)) {
					SwitchOption(validator, i - 1, true);
					Debug.Log("Was Clicked");
				}

				EditorGUILayout.EndVertical();
			}

			// Draw Right Button
			if (DrawSwitchButton(EditorIcons.ArrowRight)) { SwitchOption(validator, 1); }

			EditorGUILayout.EndHorizontal();
		}

		void DrawBoxForSelectedObject(Rect rect) {
			Vector2 SizeToAdd = new Vector2(4f, 6f);
			Vector2 PositionToAdd = new Vector2(-2f, -2.3f);

			rect.x += PositionToAdd.x;
			rect.y += PositionToAdd.y;
			rect.size += SizeToAdd;
			EditorGUI.DrawRect(rect, Color.green);
		}

		bool DidClickOccur(Rect rect) {
			var e = Event.current;
			if (e.type != EventType.MouseUp) { return false; }
			return rect.Contains(e.mousePosition);
		}

		bool DrawSwitchButton(EditorIcon icon) {
			EditorGUILayout.BeginVertical();
			EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(0));
			bool wasButtonClicked = SirenixEditorGUI.IconButton(icon);
			EditorGUILayout.GetControlRect(GUILayout.Height(5), GUILayout.Width(0));
			EditorGUILayout.EndVertical();

			return wasButtonClicked;
		}

		void SwitchOption(UpgradeValidator validator, int i, bool forcedValue = false) {
			Selection.activeGameObject = validator.gameObject;
			var currentSettings = validator.ObjectSettings.Find(x => x.Prefab == validator.CurrentObject);
			var currentIndex = validator.ObjectSettings.IndexOf(currentSettings);
			if (!forcedValue) {
				currentIndex += i;
				if (currentIndex < 0) { currentIndex = validator.ObjectSettings.Count - 1; }
				else if (currentIndex >= validator.ObjectSettings.Count) { currentIndex = 0; }
			}
			else {
				if (currentIndex == i) { return; }
				currentIndex = i;
			}

			GameObject go = validator.ObjectSettings[currentIndex].Prefab;
			validator.ApplyUpgrade(go, true);

			EditorUtility.SetDirty(validator.gameObject);
		}

	}
}
#endif