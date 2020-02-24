using System.Collections;
using System.Collections.Generic;
using Objects.Functionality;
using UnityEngine;

namespace Objects {
	public class Door : MonoBehaviour, IInteractable {
		public bool isOpen;
		public Sprite OpenDoor;
		public Sprite ClosedDoor;

		public InteractionOptions interactionOptions => _interactionOptions;
		[SerializeField] InteractionOptions _interactionOptions = null;
		public bool isValidInteractionTarget { get; }

		public void Interact() {
			if (!isOpen) {
				isOpen = true;
				GetComponent<Collider2D>().isTrigger = true;
				GetComponent<SpriteRenderer>().sprite = OpenDoor;
				return;
			}
			if (isOpen) {
				isOpen = false;
				GetComponent<Collider2D>().isTrigger = false;
				GetComponent<SpriteRenderer>().sprite = ClosedDoor;
				return;
			}
		}

	}
}