using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {
	public float maxDoorPosition;
	public float doorSpeed = 0.1f;
	public Transform leftDoor;
	public Transform rightDoor;

	List<GameObject> ObjectsInProximity = new List<GameObject>();

	public bool AdjustSizeAutomatically = false;
	public Vector2 triggerDistance = new Vector2(1.5f, 2.5f);
	void OnValidate() { if (AdjustSizeAutomatically) { GetComponent<BoxCollider2D>().size = triggerDistance; } }

	protected Vector2 endLeftPosition;
	protected Vector2 endRightPosition;
	protected Vector2 startLeftPosition;
	protected Vector2 startRightPosition;

	// TODO: Add authorized people list.
	// Some doors will only allow authorized people to pass through.
	bool isDoorOpen => ObjectsInProximity.Count > 0;

	protected virtual void Start() {
		startLeftPosition = leftDoor.transform.position;
		startRightPosition = rightDoor.transform.position;

		endLeftPosition = new Vector2(leftDoor.position.x - maxDoorPosition, leftDoor.position.y);
		endRightPosition = new Vector2(rightDoor.position.x + maxDoorPosition, rightDoor.position.y);

		transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
		transform.GetChild(1).GetComponent<Collider2D>().enabled = false;
		gameObject.layer = 9;
	}

	protected virtual void FixedUpdate() {
		if (isDoorOpen) { OpenDoor(); }
		else { CloseDoor(); }
	}

	protected virtual void OpenDoor() {
		leftDoor.position = Vector2.Lerp(leftDoor.position, endLeftPosition, doorSpeed);
		rightDoor.position = Vector2.Lerp(rightDoor.position, endRightPosition, doorSpeed);
	}
	protected virtual void CloseDoor() {
		leftDoor.position = Vector2.Lerp(leftDoor.position, startLeftPosition, doorSpeed);
		rightDoor.position = Vector2.Lerp(rightDoor.position, startRightPosition, doorSpeed);
	}

	// TODO : Add layer mask if needed.
	// We don't need it currently since the only rigidbodies are the NPCs and the player.
	void OnTriggerEnter2D(Collider2D collision) => ObjectsInProximity.Add(collision.gameObject);
	void OnTriggerExit2D(Collider2D collision) => ObjectsInProximity.Remove(collision.gameObject);
}
