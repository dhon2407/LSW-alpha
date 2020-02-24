namespace Utilities {
	using Sirenix.OdinInspector;
	using UnityEngine;

	[DefaultExecutionOrder(int.MinValue)]
	public class ChildDisabler : MonoBehaviour {

		[PropertyOrder(99), Space, Header("Children Options")]
		[Tooltip("Should children be enabled on Awake?")]
		public bool EnableChildrenOnAwake = true;

		/// <summary>Action to be called when childrens' state is changed to active.</summary>
		protected System.Action OnEnableAction;
		/// <summary>Action to be called when childrens' state is changed to inactive.</summary>
		protected System.Action OnDisableAction;

		protected virtual void Awake() {
			SetChildrenState(true);
			if (!EnableChildrenOnAwake) { SetChildrenState(false); }
		}
		[PropertyOrder(100), Button] void EnableChildren() => SetChildrenState(true);
		[PropertyOrder(101), Button] void DisableChildren() => SetChildrenState(false);

		/// <summary>
		/// Set the active state of the children gameObject.
		/// </summary>
		/// <param name="enabled">Should they be enabled?</param>
		/// <param name="CallActions">Should actions be called?</param>
		public void SetChildrenState(bool enabled, bool CallActions = false) {
			foreach (Transform child in transform) {
				child.gameObject.SetActive(enabled);
			}

			if (!CallActions) { return; }

			// Call actions
			if (enabled) { OnEnableAction?.Invoke(); }
			else { OnDisableAction?.Invoke(); }

		}

	}
}