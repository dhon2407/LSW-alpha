using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInputModule : StandaloneInputModule
{
	public static CustomInputModule instance;

	[ShowInInspector] Dictionary<int, PointerEventData> data => m_PointerData;
	protected override void Start() {
		base.Start();
		if (!instance) {
			instance = this;
			UpdateModule();
		}
		else { DestroyImmediate(gameObject); }
	}

	public PointerEventData GetPointerEventData() => m_PointerData[kMouseLeftId];

	public static bool IsPointerBlocked(int layer) {
		// There's a Unity problem where input module sometimes doesn't initialize on the first frame.
		// This is potentially fixed by calling `UpdateModule()` in the `Start()` method.
		try {
			var list = instance.GetPointerEventData().hovered;
			foreach (var item in list) {
				if (item.layer == 30) { return true; }
			}
			return false;
		}
		catch(System.Exception e) {
			Debug.LogError("Input module sometimes doesn't initialize until the end of the first frame. I'm leaving this here in case more problems than just this arise.");
			Debug.LogError($"INPUT MODULE ERROR: {e.Message}");

			return false;
		}
	}
}
