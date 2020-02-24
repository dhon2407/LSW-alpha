using System;
using System.Collections.Generic;
using System.Linq;
using CharacterStats;
using Objects.Functionality;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Objects {

	public enum ObjectUseState { 
		/// <summary>
		/// The object is not being used.
		/// </summary>
		Unused, 
		/// <summary>
		/// Preparation, for transition to another state.
		/// <para>Typically a state to allow the use of coroutines.</para>
		/// </summary>
		Preparation, 
		/// <summary>
		/// The object is being used.
		/// </summary>
		InUse 
	}

	// TODO: Implement usage from NPCs.
	/// <summary>
	/// <para>Interface for objects that are intended to be used for multiple frames.</para>
	/// <para>For objects that are intended for a 1-time interaction, see <see cref="IInteractable"/></para>
	/// </summary>
	public interface IUseable {
		GameObject gameObject { get; }

		/// <summary>
		/// Functionality and options of an <see cref="IUseable"/>.
		/// </summary>
		UseableFunctionality useableFunctionality { get; }

		/// <summary>
		/// The action that the object is currently in progress for.
		/// </summary>
		CharacterAction CurrentAction { get; set; }

		/// <summary>
		/// <para>Attempts to set the current action.</para>
		/// <para>Will return false if the action is not valid.</para>
		/// </summary>
		SetActionStateResult TrySetCurrentAction(string ActionName);

		ObjectUseState useState { get; set; }

		/// <summary> Begin the use of the <see cref="IUseable"/> object. </summary>
		void BeginUsing();

		/// <summary> Use the object. This function is intended to be called every frame the object is in use.</summary>
		void Use();

		/// <summary> Terminate the use of the object. </summary>
		/// <param name="cancel">Was the use canceled or finished normally?</param>
		void FinishUsing(bool cancel);

	}

	namespace Functionality {
		/// <summary>
		/// Functionality and options of an <see cref="IUseable"/>.
		/// </summary>
		[HideReferenceObjectPicker]
		public class UseableFunctionality {
			public bool CanBeUsed => ObjectActions.Any(x => x.ValidateStats()); // && customFunc();
			public Vector2 PlayerStandPosition => PlayerStandPoint.position;

			[Tooltip("Where should the player be navigated towards when the use of the object begins?")]
			public Transform PlayerStandPoint;

			[Tooltip("The speed that the walk/jump animation will play on. (0 == default speed of 1.8)")]
			[Range(0, 5)] public float CustomSpeedToPosition;


			[Tooltip("Where should the player be looking towards once the use of the object begins?")]
			public CharacterOrientation characterOrientationOnUseBegin;

			[Space]

			public List<CharacterAction> ObjectActions = new List<CharacterAction>();

			public CharacterAction GetActionWithName(string Name) {
				var action = ObjectActions.FirstOrDefault(x => x.ActionName == Name);
				if (action == null) { Debug.LogError($"No action with matching name: {Name} found in list."); }

				return action;
			}

		}

		/// <summary>
		/// The result of the SetAction command.
		/// <para>Implicitly casts to a <see langword="bool"/>.</para>
		/// </summary>
		public struct SetActionStateResult {
			public bool Success;
			public string ErrorMessage;

			public SetActionStateResult(bool success, string message) {
				Success = success;
				ErrorMessage = message;
			}

			/// <summary>Prints the error message to the screen.</summary>
			public void PrintErrorMessage() => GameLibOfMethods.CreateFloatingText(ErrorMessage, 1);

			public static implicit operator SetActionStateResult((bool success, string message) result) => new SetActionStateResult(result.success,result.message);
			public static implicit operator bool(SetActionStateResult state) => state.Success;
		}
	}
}