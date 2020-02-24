using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Threading;
using System;

public static class MECExtensionMethods {

	/// <summary>
	/// Starts the coroutine and returns the handle which you can await using <see cref="Timing.WaitUntilDone(CoroutineHandle)"/>.
	/// </summary>
	public static CoroutineHandle Start(this IEnumerator<float> mec) => Timing.RunCoroutine(mec);
	/// <summary>
	/// Starts the coroutine and returns the handle which you can await using <see cref="Timing.WaitUntilDone(CoroutineHandle)"/>.
	/// </summary>
	public static CoroutineHandle Start(this IEnumerator<float> mec, Segment segment) => Timing.RunCoroutine(mec, segment);
	/// <summary>
	/// Starts the coroutine and returns the handle which you can await using <see cref="Timing.WaitUntilDone(CoroutineHandle)"/>.
	/// </summary>
	public static CoroutineHandle Start(this IEnumerator<float> mec, Segment segment, string tag) => Timing.RunCoroutine(mec, segment, tag);

	/// <summary>
	/// Cancels this coroutine when the supplied game object is destroyed or made inactive.
	/// </summary>
	/// <param name="coroutine">The coroutine handle to act upon.</param>
	/// <param name="gameObject">The GameObject to test.</param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject) {
		while (Timing.MainThread != Thread.CurrentThread || (gameObject && gameObject.activeInHierarchy && coroutine.MoveNext()))
			yield return coroutine.Current;
	}

	/// <summary>
	/// Makes the coroutine terminate if the specified condition occurs.
	/// </summary>
	/// <param name="coroutine">The <typeparamref name="coroutine"/>.</param>
	/// <param name="condition">The condition for the coroutine to terminate.</param>
	/// <param name="callback">The callback that will occur if the coroutine terminates.</param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> CancelIf(this IEnumerator<float> coroutine, System.Func<bool> condition, System.Action callback = null) {
		while (true) {
			if (condition()) {
				callback?.Invoke();
				yield break;
			}

			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		}
	}

	/// <summary>
	/// Adds an action that is called each frame the coroutine is running.
	/// </summary>
	/// <param name="coroutine">The <typeparamref name="coroutine"/>.</param>
	/// <param name="action">Action to be called each frame while the coroutine is running.</param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> AddActionEachFrame(this IEnumerator<float> coroutine, System.Action action) {
		while (true) {
			action?.Invoke();
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		}
	}

	/// <summary>
	/// Delays the coroutine's execution until <typeparamref name="prependCoroutine"/> finishes.
	/// </summary>
	/// <param name="coroutine">The <typeparamref name="coroutine"/>.</param>
	/// <param name="prependCoroutine">The coroutine to run first.</param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> PrependWith(this IEnumerator<float> coroutine, IEnumerator<float> prependCoroutine) {
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		} 
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return prependCoroutine.Current; }
			else if (prependCoroutine.MoveNext()) { yield return prependCoroutine.Current; }
			else { break; }
		}
	}

	/// <summary>
	/// Queues a <typeparamref name="coroutineCallback"/> to run after <typeparamref name="coroutine"/> has finished.
	/// </summary>
	/// <param name="coroutine">The <typeparamref name="coroutine"/>.</param>
	/// <param name="coroutineCallback">The coroutine to be executed after <typeparamref name="coroutine"/> has finished. </param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> AppendWith(this IEnumerator<float> coroutine, IEnumerator<float> coroutineCallback) {
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		}
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutineCallback.Current; }
			else if (coroutineCallback.MoveNext()) { yield return coroutineCallback.Current; }
			else { break; }
		}
	}

	/// <summary>
	/// Queues a <typeparamref name="callback"/> action to call after <typeparamref name="coroutine"/> has finished.
	/// </summary>
	/// <param name="coroutine">The <typeparamref name="coroutine"/>.</param>
	/// <param name="callback">The coroutine to be executed after <typeparamref name="coroutine"/> has finished. </param>
	/// /// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> AddCallback(this IEnumerator<float> coroutine, System.Action callback) {
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		}
		callback?.Invoke();
	}

	/// <summary>
	/// Executes the coroutine after a certain delay.
	/// </summary>
	/// <param name="coroutine"></param>
	/// <param name="delay">The delay after which the <typeparamref name="coroutine"/> is called.</param>
	/// <returns>The modified coroutine handle.</returns>
	public static IEnumerator<float> AddDelay(this IEnumerator<float> coroutine, float delay) {
		yield return Timing.WaitForSeconds(delay);
		while (true) {
			if (Timing.MainThread != Thread.CurrentThread) { yield return coroutine.Current; }
			else if (coroutine.MoveNext()) { yield return coroutine.Current; }
			else { break; }
		}
	}

	/// <summary>
	/// Calls an action with the delay.
	/// <para>Optional parameters for cancel condition, repetitive action call per frame until the delay happens, and immediate quit condition</para>
	/// </summary>
	/// <param name="actionToCall">Action to be called after the delay.</param>
	/// <param name="Delay">Delay which the action will be called after.</param>
	/// <param name="ActionPerFrameUntilCall">Action to be called each frame until the delay.</param>
	/// <param name="ConditionToCancel">Condition in which the action will be called immediately.</param>
	/// <param name="ConditionToQuit">Condition in which the action will not be called at all.</param>
	public static void CallDelayed(float Delay, Action actionToCall, Action ActionPerFrameUntilCall = null, Func<bool> ConditionToCancel = null, Func<bool> ConditionToQuit = null) {
		DelayedCall(actionToCall, Delay, ActionPerFrameUntilCall, ConditionToCancel, ConditionToQuit).Start();
	}

	static IEnumerator<float> DelayedCall(Action actionToCall, float Delay, Action ActionPerFrameUntilCall = null, Func<bool> ConditionToCancel = null, Func<bool> ConditionToQuit = null) {
		float t = Delay;

		bool hasCancelCondition = ConditionToCancel != null;
		bool hasQuitCondition = ConditionToQuit != null;
		bool callsActionEachFrame = ActionPerFrameUntilCall != null;

		while (true) {
			t -= Time.deltaTime;
			if (t <= 0) { break; }
			if (hasCancelCondition && ConditionToCancel()) { break; }
			if (hasQuitCondition && ConditionToQuit()) { yield break; }

			if (callsActionEachFrame) { ActionPerFrameUntilCall(); }

			yield return 0f;
		}

		actionToCall?.Invoke();
	}

}