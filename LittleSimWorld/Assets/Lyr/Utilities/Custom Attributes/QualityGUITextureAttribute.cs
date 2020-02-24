using System;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class DrawTextureDrawer : OdinAttributeDrawer<DrawTextureAttribute> {

	protected override void DrawPropertyLayout(GUIContent label) {
		try {
			var texture = (Texture2D) Property.ValueEntry.WeakSmartValue;
			CallNextDrawer(label);
			var rect = EditorGUILayout.GetControlRect(GUILayout.Width(Attribute.Width), GUILayout.Height(Attribute.Height));
			GUI.DrawTexture(rect, texture);
		}
		catch {
			UnityEditor.EditorGUILayout.HelpBox("[DrawTexture] is only usable on Textures.", MessageType.Error);
			CallNextDrawer(label);
		}
	}
}

#endif
public class DrawTextureAttribute : Attribute {
	public int Width, Height;
	public DrawTextureAttribute(int Size) => Width = Height = Size;
	public DrawTextureAttribute(int Width, int Height) {
		this.Width = Width;
		this.Height = Height;
	}
}
