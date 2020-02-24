using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This component marks objects to be enabled/disabled together with the state.
/// </summary>
public class ObjectStateChangerState : SerializedStateMachineBehaviour {

	[ValueDropdown("GetValues", IsUniqueList = true, ExpandAllMenuItems = true)]
	public List<string> ObjectsToChangeState = new List<string>();

	[Tooltip("Should the object be disabled on the beginning?")]
	public bool DisableOnBeginningInstead;

	[Tooltip("Should the occluder also be disabled?")]
	public bool AlsoDisableOccluder = true;

	/// TODO: Make a dropdown of all animator parameters appear.
	//[Tooltip("ASDF"), ValueDropdown("GetAnimatorParameters")]
	string ConditionalValue = default;


	static GameObject Occluder;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (ObjectsToChangeState.Count == 0) {
			Debug.LogWarning($"Object list for {animator.GetNextAnimatorClipInfo(layerIndex)[0].clip.name} is empty.");
		}

		foreach (string objName in ObjectsToChangeState) {
			GameObject obj = AnimatorStatesHelper.GetObjectWithName(objName);
			obj.SetActive(!DisableOnBeginningInstead);
		}

		if (!Occluder) { Occluder = AnimatorStatesHelper.GetOccluder(); }
		if (AlsoDisableOccluder) { Occluder.SetActive(false); }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		foreach (string objName in ObjectsToChangeState) {
			GameObject obj = AnimatorStatesHelper.GetObjectWithName(objName);
			obj.SetActive(DisableOnBeginningInstead);
		}
		if (AlsoDisableOccluder) { Occluder.SetActive(true); }
	}



	class AnimatorParameter<T> {
		public string Name = default;
		public T Value = default;
	}

#if UNITY_EDITOR

	// for In-Editor use
	ValueDropdownList<string> GetValues() {
		var list = new ValueDropdownList<string>();
		foreach (var pair in AnimatorStatesHelper.instance.References) {
			list.Add(pair.Value.name, pair.Key);
		}
		return list;
	}

	ValueDropdownList<string> GetAnimatorParameters() {
		var list = new ValueDropdownList<string>();
		var anim = GetCurrentController();

		if (!anim) { list.Add(ConditionalValue); }
		else {
			foreach (var param in anim.parameters) {
				dynamic newParam;

				switch (param.type) {
					case AnimatorControllerParameterType.Float:
						newParam = new AnimatorParameter<float>();
						break;
					case AnimatorControllerParameterType.Int:
						newParam = new AnimatorParameter<int>();
						break;
					case AnimatorControllerParameterType.Bool:
						newParam = new AnimatorParameter<bool>();
						break;
					case AnimatorControllerParameterType.Trigger:
						continue;
					default:
						continue;
				}

				list.Add(newParam, param.name);
			}
		}

		return list;
	}


	static UnityEditor.Animations.AnimatorController GetCurrentController() {
		UnityEditor.Animations.AnimatorController controller = null;
		var tool = UnityEditor.EditorWindow.focusedWindow;
		var toolType = tool.GetType();

		var controllerProperty = toolType.GetProperty("animatorController", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
		if (controllerProperty != null) { controller = controllerProperty.GetValue(tool, null) as UnityEditor.Animations.AnimatorController; }
		else { Debug.Log("EditorWindow.focusedWindow " + tool + " does not contain animatorController", tool); }

		return controller;
	}
#endif
}