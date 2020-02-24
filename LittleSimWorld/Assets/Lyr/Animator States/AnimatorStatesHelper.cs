using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimatorStatesHelper : SerializedMonoBehaviour {

	//[InfoBox("Assign object references here to be able to select them on the Animator state")]
	public Dictionary<string, GameObject> References;

	public GameObject Occluder;

	static AnimatorStatesHelper _instance;
	public static AnimatorStatesHelper instance {
		get {
			if (_instance == null) { _instance = FindObjectOfType<AnimatorStatesHelper>(); }
			return _instance;
		}
	}

	public static GameObject GetOccluder() => instance.Occluder;

	public static GameObject GetObjectWithName(string Name) {
		try { return instance.References[Name]; }
		catch { Debug.LogError($"{Name} not found in dictionary"); return null; }
	}
}
