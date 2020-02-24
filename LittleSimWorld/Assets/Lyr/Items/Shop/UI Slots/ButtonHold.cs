namespace LSW.Helpers {
    using System.Collections.Generic;
    using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	public class ButtonHold : Button {
		[Header("Hold Action Triggers")]
		[SerializeField] private float holdTriggerTime = 0.5f;
		[SerializeField] private float triggerRate = 0.1f;

		private bool _holdActionStarted = false;
		private bool _holdActionOngoing = false;

		private void Update() {
			if (!_holdActionStarted && IsPressed()) {
				StartHoldActions().CancelWith(gameObject).Start();
			}
		}

		private IEnumerator<float> StartHoldActions() {
			_holdActionStarted = true;
			yield return MEC.Timing.WaitForSeconds(holdTriggerTime);

			_holdActionOngoing = true;
			while (IsPressed()) {
				onClick.Invoke();
				yield return MEC.Timing.WaitForSeconds(triggerRate);
			}

			_holdActionOngoing = false;
			_holdActionStarted = false;
		}

		public override void OnPointerClick(PointerEventData eventData) {
			if (eventData.button != PointerEventData.InputButton.Left) { return; }
			if (_holdActionOngoing || !IsActive() || !IsInteractable()) { return; }

			UISystemProfilerApi.AddMarker("Button.onClick", this);
			onClick.Invoke();
		}

	}
}