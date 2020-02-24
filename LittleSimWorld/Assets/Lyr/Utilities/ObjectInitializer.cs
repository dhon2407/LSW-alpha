#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using InventorySystem;
using Objects;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class ObjectInitializer : SerializedMonoBehaviour
{
	[Button]
	void InitializeObjects() {

		var furnitureAssetList = new List<Items.FurnitureItemData>(100);

		var assetGUIDs = AssetDatabase.FindAssets("t:FurnitureItemData");
		foreach (var GUID in assetGUIDs) {
			var path = AssetDatabase.GUIDToAssetPath(GUID);
			var asset = AssetDatabase.LoadAssetAtPath<Items.FurnitureItemData>(path);
			furnitureAssetList.Add(asset);
		}

		var objects = FindObjectsOfType<BaseObject>();
		foreach (var obj in objects) {
			var tooltip = obj.GetComponentInChildren<LSW.Tooltip.GameObjectTooltipArea>();
			if (!tooltip) { Debug.Log("Couldn't find tooltip for " + obj.name); continue; }
			var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj.gameObject);
			foreach (var furnData in furnitureAssetList) {
				if (furnData.LinkedPrefab == prefab) {
					tooltip.tooltipDescription = furnData.Name;
					Debug.Log($"Updated tooltip for {obj.name}. ({furnData.Name})");
					break;
				}
			}
		}




		return;

		//foreach (var obj in objects) {
		//
		//	if (!obj.transform.Find("Helpers")) {
		//		var helpers = new GameObject("Helpers").transform;
		//		helpers.SetParent(obj.transform);
		//		new GameObject("PathfindingPoint").transform.SetParent(helpers);
		//		//new GameObject("Player Stand Point").transform.SetParent(helpers);
		//		new GameObject("MouseOverArea").transform.SetParent(helpers);
		//	}
		//	else { return; }
		//	//obj.useableFunctionality.CustomSpeedToPosition = 0;
		//	//obj.useableFunctionality.PlayerStandPoint = obj.transform.Find("Helpers").Find("Player Stand Point");
		//	//obj.useableFunctionality.characterOrientationOnUseBegin = CharacterOrientation.Top;
		//
		//	obj.interactionOptions.PlayerPathfindingTarget = obj.transform.Find("Helpers").Find("PathfindingPoint");
		//	obj.interactionOptions.MouseOverCollider = obj.transform.Find("Helpers").Find("MouseOverArea");
		//	obj.interactionOptions.InteractionRange = 0.75f;
		//	obj.interactionOptions.InteractionCenterOffset = new Vector2(0, 0.5f);
		//}
	}

}
#endif