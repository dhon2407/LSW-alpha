using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectHider : MonoBehaviour
{
	[Header("Toggle this to view and edit the objects of this tilemap")]
	[System.NonSerialized, ShowInInspector] 
	public bool ShowObjects = false;

	void OnValidate() {
		if (ShowObjects) { transform.Find("Objects").gameObject.hideFlags = HideFlags.None; }
		else { transform.Find("Objects").gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable; }
	}

}
