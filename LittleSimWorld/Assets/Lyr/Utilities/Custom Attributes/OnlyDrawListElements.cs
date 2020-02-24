using System;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

public class DrawChildElementsAttributeDrawer : OdinAttributeDrawer<DrawChildElementsAttribute> {

	protected override void DrawPropertyLayout(GUIContent label) {
		if (Attribute.Title != null) {
			SirenixEditorGUI.Title(Attribute.Title, null, TextAlignment.Center, true, true);
		}

		foreach (var p in this.Property.Children) { p.Draw(p.Label); }
	}
}

[DontApplyToListElements]
#endif
public class DrawChildElementsAttribute : Attribute {
	public string Title = null;
	public DrawChildElementsAttribute() { }
	public DrawChildElementsAttribute(string Title) { this.Title = Title; }
}
