using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class TitleFromMethodAttributeDrawer : OdinAttributeDrawer<TitleFromMethodAttribute> {

	MethodInfo del;
	string error;
	object valueEntry;
	enum _MethodPlace { None, FoundOnParent, FoundInClass }
	_MethodPlace _place;

	protected override void Initialize() {
		// Scan for methods on type's class/struct.
		if (MemberFinder.Start(this.Property.ValueEntry.TypeOfValue)
				.IsMethod()
				.IsNamed(Attribute.Method)
				.HasReturnType<string>()
				.TryGetMember<MethodInfo>(out del,out error)) {

			_place = _MethodPlace.FoundInClass;
		}
		// Scan for methods on parent type's class/struct.
		else if (MemberFinder.Start(this.Property.ValueEntry.ParentType)
				.IsMethod()
				.IsNamed(Attribute.Method)
				.HasReturnType<string>()
				.TryGetMember<MethodInfo>(out del,out error)) {

			_place = _MethodPlace.FoundOnParent;
		}
	}

	protected override void DrawPropertyLayout(GUIContent label) {

		// Don't draw added emtpy space for the first property.
		if (Property != Property.Tree.GetRootProperty(0)) {
			EditorGUILayout.Space();
		}

		if (label == null) { label = Property.Label; }

		if (_place == _MethodPlace.FoundInClass) {
			valueEntry = Property.ValueEntry.WeakSmartValue;
		}
		else if (_place == _MethodPlace.FoundOnParent) {
			valueEntry = Property.Parent.ValueEntry.WeakSmartValue;
		}

		if (error != null) {
			SirenixEditorGUI.ErrorMessageBox(error);
		}
		else {
			var title = (string) del.Invoke(valueEntry,null);
			SirenixEditorGUI.Title(title,null,TextAlignment.Center,true,true);
		}

		CallNextDrawer(label);
	}
}

#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class,AllowMultiple = true,Inherited = true)]
[DontApplyToListElements]
public class TitleFromMethodAttribute : Attribute {
	public string Method;
	public TitleFromMethodAttribute(string Method) {
		this.Method = Method;

	}
}
