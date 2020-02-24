using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

	[SerializeField] Image _progressBar = null;
	[SerializeField] TMPro.TextMeshProUGUI _text = null;

	public static float CurrentValue { get; private set; }
	public static float MaxValue { get; private set; }

	public static float CurrentProgress => CurrentValue / MaxValue;

	static ProgressBar instance;
	static int CurrentID;
	Canvas canvas;
	CoroutineHandle? currentlyTracking;

	void Awake() {
		instance = this;
		canvas = GetComponent<Canvas>();
		canvas.enabled = false;
	}

	/// <summary>
	/// Opens the progress bar UI and provides a tracker ID for <see cref="HideUI"/>.
	/// </summary>
	/// <returns>Tracker ID for <see cref="HideUI"/></returns>
	public static int ShowUI() {
		instance.canvas.enabled = true;
		if (CurrentID == int.MaxValue) { CurrentID = int.MinValue; }
		return ++CurrentID;
	}

	/// <summary>
	/// Hide the UI with the specified ID. Will do nothing if the ID is not the active ID.
	/// </summary>
	/// <param name="ID">The ID generated from <see cref="ShowUI"/> or a StartTracking method.</param>
	public static void HideUI(int ID) {
		if (CurrentID != ID) { return; }
		CurrentID++;

		instance.currentlyTracking = null;
		instance.canvas.enabled = false;
		CurrentValue = 0;
		MaxValue = 0;
	}

	#region Automatic tracking

	/// <summary>
	/// Starts tracking an action with given Funcs, and automatically updates its progress.
	/// <para>Call <see cref="HideUI"/> with the ID returned from this method as a parameter to terminate the tracking.</para>
	/// </summary>
	/// <param name="Title">The title of the progress bar.</param>
	/// <param name="currentValueFunc">The function which outputs the CurrentValue float.</param>
	/// <param name="maxValueFunc">The function which outputs the MaxValue float.</param>
	/// <param name="OnCompleteCallback">The function to be called on completion.</param>
	public static int StartTracking(string Title, System.Func<float> currentValueFunc, System.Func<float> maxValueFunc, System.Action OnCompleteCallback = null) {

		if (instance.currentlyTracking.HasValue) { Debug.LogWarning("Current value for the Progress bar has been overriden."); }

		instance._text.text = Title;

		CurrentValue = currentValueFunc();
		MaxValue = maxValueFunc();

		instance.canvas.enabled = true;
		if (CurrentID == int.MaxValue) { CurrentID = int.MinValue; }
		CurrentID++;

		instance.currentlyTracking = KeepTrack(currentValueFunc, maxValueFunc, OnCompleteCallback).Start();

		return CurrentID;
	}

	/// <summary>
	/// Starts tracking an action with given Funcs, and automatically updates its progress.
	/// <para>Call <see cref="HideUI"/> with the ID returned from this method as a parameter to terminate the tracking.</para>
	/// </summary>
	/// <param name="Title">The title of the progress bar.</param>
	/// <param name="currentValueFunc">The function which outputs the CurrentValue float.</param>
	/// <param name="maxValue">The max value of the progress bar.</param>
	/// <param name="OnCompleteCallback">The function to be called on completion.</param>
	public static int StartTracking(string Title, System.Func<float> currentValueFunc, float maxValue, System.Action OnCompleteCallback = null) => StartTracking(Title, currentValueFunc, () => maxValue, OnCompleteCallback);

	/// <summary>
	/// Starts tracking an action with progress func, and automatically updates its progress.
	/// <para>Progress output should be clamped between 0 and 1.</para>
	/// <para>Call <see cref="HideUI"/> with the ID returned from this method as a parameter to terminate the tracking.</para>
	/// </summary>
	/// <param name="Title">The title of the progress bar.</param>
	/// <param name="ProgressFunc">The function which outputs the CurrentProgress float.</param>
	/// <param name="OnCompleteCallback">The function to be called on completion.</param>
	public static int StartTracking(string Title, System.Func<float> ProgressFunc, System.Action OnCompleteCallback = null) => StartTracking(Title, ProgressFunc, () => 1, OnCompleteCallback);

	static IEnumerator<float> KeepTrack(System.Func<float> currentValueFunc, System.Func<float> maxValueFunc, System.Action OnCompleteCallback) {
		int cachedID = CurrentID;

		while (CurrentID == cachedID) {
			CurrentValue = currentValueFunc();
			MaxValue = maxValueFunc();

			if (CurrentValue >= MaxValue) {
				HideUI(CurrentID);
				OnCompleteCallback?.Invoke();
				instance.currentlyTracking = null;
			}
			else { UpdateVisuals(); }
			yield return 0f;
		}
	}
	#endregion

	#region Manual UI Update

	/// <summary>
	/// Manually sets the title for the progress bar with the specified ID.
	/// </summary>
	/// <param name="Title">Title of the progress bar.</param>
	/// <param name="ID">ID that was generated from <see cref="ShowUI"/>.</param>
	public static bool SetTitle(string Title, int ID) {
		if (CurrentID != ID) {
			Debug.LogError("ID provided is different than Current ID.");
			return false;
		}
		instance._text.text = Title;
		return true;
	}

	/// <summary>
	/// Manually sets the progress for the progress bar with the specified ID.
	/// <para>Value must be clamped between 0 and 1.</para>
	/// </summary>
	/// <param name="value">Percentage of the progress bar.</param>
	/// <param name="ID">ID that was generated from <see cref="ShowUI"/>.</param>
	/// <returns></returns>
	public static bool SetProgress(float value, int ID) {
		if (CurrentID != ID) {
			Debug.LogError("ID provided is different than Current ID.");
			return false;
		}

		MaxValue = 1;
		CurrentValue = value;

		UpdateVisuals();
		return true;
	}

	#endregion

	static void UpdateVisuals() => instance._progressBar.fillAmount = (MaxValue == 0) ? 0 : Mathf.Clamp(CurrentValue / MaxValue, 0, 1);
}
