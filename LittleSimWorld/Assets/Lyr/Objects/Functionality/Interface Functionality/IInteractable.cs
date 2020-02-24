using Objects.Functionality;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Objects {
	/// <summary> Interface for objects that can interacted with, via the mouse or by pressing the interact button in-game.
	/// <para>For an example on how to implement this, see <see cref="InteractableObject"/>.</para>
	/// </summary>
	public interface IInteractable {

		GameObject gameObject { get; }

		/// <summary>
		/// <para>Functionality of an <see cref="IInteractable"/> object.</para>
		/// <para>Override the <see cref="InteractionOptions"/> class for custom functionality.</para>
		/// </summary>
		InteractionOptions interactionOptions { get; }

		/// <summary>
		/// Determines if the target is available for interaction.
		/// </summary>
		bool isValidInteractionTarget { get; }

		/// <summary> 
		/// <para>The method that is called when the interaction occurs. </para>
		/// <para>This is called right after the character presses E, or finishes the path set via pathfinding, if interacted via mouse.</para>
		/// </summary>
		void Interact();
	}

	namespace Functionality {

		/// <summary>
		/// <para>Interaction options of an <see cref="IInteractable"/> object.</para>
		/// </summary>
		[HideReferenceObjectPicker]
		public class InteractionOptions {

			[Tooltip("The object that has the collider that the MouseOver will trigger")]
			public Transform MouseOverCollider;

			[Space]

			[Tooltip("The range which the interaction can begin. (0 = disabled)\n This may override the Player Pathfinding Target when interacting via mouse.")]
			[Range(0, 5)] public float InteractionRange;

			[Tooltip("Offset from the transform's pivot for the interaction")]
			public Vector2 InteractionCenterOffset;

			[Space]

			[Tooltip("Where should the player be navigated towards when he clicks on the object from distance?")]
			public Transform PlayerPathfindingTarget;
			public Vector2 PlayerStandPosition => PlayerPathfindingTarget.position;


			[Tooltip("Should the player face towards an roientation once the interaction begins?")]
			[FitLabelWidth] public bool UpdateOrientationOnTargetReach;

			[Tooltip("Where should the player be looking towards once the interaction begins?")]
			[ShowIf("UpdateOrientationOnTargetReach")] public CharacterOrientation TargetOrientation;

			[System.Obsolete("Isn't supported by current highlighting system??")]
			[Tooltip("The color of the highlighting when the mouse is over the object, or the player is within proximity.")]
			public Color HighlightColor;

			/// <summary>
			/// Make character face the orientation selected in the editor.
			/// <para>Intended to use right before calling <see cref="IInteractable.Interact"/></para>
			/// </summary>
			public void UpdateCharacterOrientation() {
				if (!UpdateOrientationOnTargetReach) { }
				else { SpriteControler.Instance.UpdateCharacterOrientation(TargetOrientation); }
			}

			#region Editor Functionality
#if UNITY_EDITOR

			[Button, ShowIf("@!MouseOverCollider || MouseOverCollider.GetComponent<Collider2D>() == null"), PropertyOrder(-1)]
			[InfoBox("Initialize the selected transform to contain a collider matching the shape of the object, and place it on the appropriate layer.\n\n" +
					 "Note: Place empty transform and hit the button.\nThe object's collider might need retouching to finalize.")]
			void InitializeCollider() {
				if (!MouseOverCollider) {
					Debug.LogError("MouseOverCollider is not assigned.");
					return;
				}

				var parent = MouseOverCollider.parent;
				var spr = parent.GetComponent<SpriteRenderer>();
				if (!spr) {
					parent = MouseOverCollider.parent.parent;
					spr = parent.GetComponent<SpriteRenderer>();
					if (!spr) {
						Debug.LogError("MouseOverCollider must be a grandchild or child of the SpriteRenderer in the hiearchy.");
						return;
					}
				}
				var tempCollider = parent.gameObject.AddComponent<BoxCollider2D>();
				var newCollider = MouseOverCollider.gameObject.AddComponent<BoxCollider2D>();
				newCollider.size = tempCollider.size;
				newCollider.offset = tempCollider.offset;

				Object.DestroyImmediate(tempCollider);
				MouseOverCollider.gameObject.layer = LayerMask.NameToLayer("MouseOverCollider");
				MouseOverCollider.localPosition = Vector3.zero;
			}
#endif
			#endregion
		}

	}
}