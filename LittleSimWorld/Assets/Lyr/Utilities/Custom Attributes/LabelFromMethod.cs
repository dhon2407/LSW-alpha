using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Reflection;
using UnityEngine;

[DrawerPriority(DrawerPriorityLevel.SuperPriority)]
public class LabelFromMethodAttributeDrawer : OdinAttributeDrawer<LabelFromMethodAttribute> {

	MethodInfo del;
	string error;
	object valueEntry;
	enum _MethodPlace { None, FoundOnParent, FoundInClass }
	_MethodPlace _place;

	protected override void Initialize() {
		// Scan for methods on type's class/struct.
		if (MemberFinder.Start(Property.ValueEntry.TypeOfValue)
				.IsMethod()
				.IsNamed(Attribute.Method)
				.HasReturnType<string>()
				.TryGetMember<MethodInfo>(out del,out error)) {

			_place = _MethodPlace.FoundInClass;
		}
		// Scan for methods on parent type's class/struct.
		else if (MemberFinder.Start(Property.ValueEntry.ParentType)
				.IsMethod()
				.IsNamed(Attribute.Method)
				.HasReturnType<string>()
				.TryGetMember<MethodInfo>(out del,out error)) {

			_place = _MethodPlace.FoundOnParent;
		}
	}

	protected override void DrawPropertyLayout(GUIContent label) {
		if (label == null) { label = Property.Label; }

		if (_place == _MethodPlace.FoundInClass) {
			valueEntry = Property.ValueEntry.WeakSmartValue;
		}
		else if (_place == _MethodPlace.FoundOnParent) {
			valueEntry = Property.Parent.ValueEntry.WeakSmartValue;
		}

		if (error != null) { SirenixEditorGUI.ErrorMessageBox(error); }
		else { label.text = (string) del.Invoke(valueEntry,null); }

		CallNextDrawer(label);
	}
}
#endif



[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class,AllowMultiple = true,Inherited = true)]
[DontApplyToListElements]
public class LabelFromMethodAttribute : Attribute {
	public string Method;
	public LabelFromMethodAttribute(string Method) { this.Method = Method; }
}
