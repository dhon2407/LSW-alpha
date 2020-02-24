#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[TypeInfoBox("This object will pop up warnings whenever the scene is marked as dirty.")]
public class DirtySceneInvestigator : MonoBehaviour {

	static DirtySceneInvestigator() {
		Undo.postprocessModifications += OnPostProcessModifications;
	}

	private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications) {
		Debug.LogWarning($"Scene was marked Dirty by number of objects = {propertyModifications.Length}");
		for (int i = 0; i < propertyModifications.Length; i++) {
			Debug.LogWarning($"currentValue.target = {propertyModifications[i].currentValue.target}", propertyModifications[i].currentValue.target);
		}
		return propertyModifications;
	}
}

#endif
