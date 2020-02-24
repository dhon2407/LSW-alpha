using System.Collections.Generic;
using HighlightPlus2D;
using InventorySystem;
using UnityEngine;
using Objects;

[DefaultExecutionOrder(-1)]
public class InteractionChecker : MonoBehaviour {
	public KeyCode KeyToInteract;
	public static InteractionChecker Instance;
	public AnimationCurve jumpCurve;
    private bool interactCD;

	[System.NonSerialized] public GameObject lastHighlightedObject_Closest;
	[System.NonSerialized] public GameObject lastHighlightedObject_Mouse;

	public float JumpSpeed = 1.8f; // Per Second

	public ContactFilter2D contactFilter;
	public ContactFilter2D contactFilterForClosest;

	Camera mainCamera;
	List<Collider2D> colliders = new List<Collider2D>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

	void Start() {
		mainCamera = Camera.main;
	}

	void Update() {
        if (Input.GetKeyUp(KeyToInteract)) {
			if (GameLibOfMethods.isSleeping || !GameLibOfMethods.canInteract || GameLibOfMethods.doingSomething) { return; }
			GameObject interactableObject = CheckClosestInteractable();
			if (interactableObject) { InteractWith(interactableObject); }

		}

		ApplyHighlights();
	}


	void ApplyHighlights() {
		HighlightClosest();
		//HighlightMouseOver();
	}

	void HighlightClosest() {
		if (!GameLibOfMethods.player || GameLibOfMethods.doingSomething || !GameLibOfMethods.canInteract) {
			if (lastHighlightedObject_Closest) { lastHighlightedObject_Closest = null; }
			HighlightManager2D.instance.SwitchesCollider(null);
			HighlightManager2D.instance.baseEffect.SetHighlighted(false);
			return;
		}


		// Check for CLOSEST Interactables
		GameObject highlightedObject = CheckClosestInteractable();

		if (highlightedObject) {
			if (lastHighlightedObject_Closest && lastHighlightedObject_Closest != highlightedObject) {
				HighlightManager2D.instance.baseEffect.SetHighlighted(false);
			}
			HighlightManager2D.instance.SwitchesCollider(highlightedObject.transform);
			lastHighlightedObject_Closest = highlightedObject;
		}
		else if (lastHighlightedObject_Closest) {
			lastHighlightedObject_Closest = null;
		}
		else {
			HighlightManager2D.instance.SwitchesCollider(null);
			HighlightManager2D.instance.baseEffect.SetHighlighted(false);
		}
	}
	public void InteractWith(GameObject interactableObject) {

		GameTime.Clock.ResetSpeed();
		if (GameTime.Clock.Paused) return;
		
		Debug.Log("Interacting");
		if (!interactableObject) {
			Debug.LogWarning("InteractWith() called with null parameters.");
		}
		else if (interactableObject.GetComponent<IInteractable>() != null)
        {
            //henrique - added the option to check if the Interface that displays options in a menu
            if (interactableObject.gameObject.GetComponent<ImenuInteraction>() != null)
            {
                interactableObject.gameObject.GetComponent<ImenuInteraction>().OpenMenu();
            }
            else
            {
                PlayerAnimationHelper.ResetAnimations();

                var interactable = interactableObject.GetComponent<IInteractable>();
                System.Action Interact = interactable.Interact;
                FindObjectOfType<PathFinding.PlayerPathfinding>().MoveTo(interactable.interactionOptions.PlayerStandPosition, 2, Interact);
                GameLibOfMethods.doingSomething = true;
                StartCoroutine(DelayInteractionMenu(interactable, interactable.interactionOptions.PlayerStandPosition, Interact));
            }

		}
		else if (interactableObject.GetComponent<Item>()) {
			Inventory.PlaceOnBag(interactableObject.GetComponent<Item>());
		}

	}

    IEnumerator<WaitForSeconds> DelayInteractionMenu(IInteractable interactable, Vector2 targetPos, System.Action interact)
    { 
        while (Vector2.Distance(Player.position, targetPos) >= .5f)
        {
            yield return new WaitForSeconds(.1f);
        }
        interactable.interactionOptions.UpdateCharacterOrientation();
        interactable.Interact();
    }
        GameObject CheckClosestInteractable() {

		int layerMask = 1 << 10 | 1 << 15 | 1 << 16;

		Vector3 playerPos = Player.transform.position;
		Vector3 facingDir = new Vector2(Player.anim.GetFloat("FaceX"), Player.anim.GetFloat("FaceY"));
		Physics2D.queriesStartInColliders = true;
		var hit = Physics2D.CircleCast(playerPos, 0.2f, facingDir, 0.5f, layerMask);
		Physics2D.queriesStartInColliders = false;

		if (hit) {
			// Validate again
			var dir = hit.transform.position - playerPos;
			//Physics2D.queriesStartInColliders = true;
			var hit2 = Physics2D.Raycast(playerPos, dir, 0.5f, 1 << 16);
			//Physics2D.queriesStartInColliders = false;
			if (hit2) { return null; }

			return hit.transform.gameObject;
		}

		return null;
	}



	#region UNUSED
	/*
	void HighlightMouseOver() {
		// Check for MOUSE Interactables
		Transform highlight_mouse = CheckMouseOverInteractable();
		if (highlight_mouse) {
			if (lastHighlightedObject_Mouse && lastHighlightedObject_Mouse != highlight_mouse) {
			}

			//highlight_mouse.isMouseOver = true;
			lastHighlightedObject_Mouse = highlight_mouse;

		}
		else if (lastHighlightedObject_Mouse) {
			//lastHighlightedObject_Mouse.isMouseOver = false;
			lastHighlightedObject_Mouse = null;
		}
	}


	Transform CheckMouseOverInteractable() {

		Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		colliders.Clear();

		Physics2D.OverlapCircle(pos, 0.1f, contactFilter, colliders);

		foreach (Collider2D collider in colliders) {
			Transform interactable = collider.transform;
			if (interactable == null) { continue; }

			return interactable;
		}

		return null;
	}
	*/
	#endregion
}
