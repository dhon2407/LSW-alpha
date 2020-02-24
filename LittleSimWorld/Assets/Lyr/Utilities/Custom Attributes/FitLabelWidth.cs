using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[DrawerPriority(0,99,0)]
public class FitLabelWidthAttributeDrawer : OdinAttributeDrawer<FitLabelWidthAttribute> {
	protected override void DrawPropertyLayout(GUIContent label) {
		if (label != null) {
			var width = EditorStyles.label.CalcSize(label).x + GUIHelper.CurrentIndentAmount + 10;
			GUIHelper.PushLabelWidth(width);
		}
		CallNextDrawer(label);
		if (label != null) { GUIHelper.PopLabelWidth(); }
	}
}
#endif

public class FitLabelWidthAttribute : Attribute { }
