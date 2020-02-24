using System;
using System.Collections.Generic;
using Objects;
using UnityEngine;

namespace InventorySystem {
	[Serializable]
	public class ItemList : InteractableObject, IInteractionOptions {
		[SerializeField] private int containerID = 0;
		[SerializeField] private int availableSlot = 6;
		[SerializeField] private List<ItemInfo> items = null;

		private bool _isOpen;
		// Must skip a frame to let the Input reset.
		// TODO: Deal with this in a better way. Maybe an input manager?
		private bool _skippedFrame;

		public int Count => items.Count;
		public int MaxSlot => availableSlot;
		public List<ItemInfo> Items => items;
		public int ID => containerID;

        public void Interact(int a)
        {
            Interact();
        }



        public override void Interact() {
			if (Inventory.Ready) {
				Inventory.OpenCloseContainer(this, gameObject.name);
				_isOpen = true;
				_skippedFrame = false;
			}
		}

		protected virtual void Update() {
			if (!_isOpen) { return; }
			if (!_skippedFrame) { _skippedFrame = true; return; }

			if (!Inventory.ContainerOpen || Inventory.currentContainer != this) { _isOpen = false; }
			else if (Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Escape)) {
				Inventory.OpenCloseContainer(this, gameObject.name);
				_isOpen = false;
			}
			else {
				Collider2D col = GetComponent<Collider2D>();

				float distanceFromPlayer = (Player.position - col.ClosestPoint(Player.position)).sqrMagnitude;
				if (distanceFromPlayer <= 0.5f) { return; }

				Inventory.OpenCloseContainer(this, gameObject.name);
				_isOpen = false;
			}
		}

		[Serializable]
		public struct ItemInfo {
			public int count;
			public ItemCode itemCode;
		}

		public void UpdateItems(List<ItemInfo> newItems) => items = newItems;
	}
}
