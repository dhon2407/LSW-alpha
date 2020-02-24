using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClock = GameTime.Clock;

public class SingleSlidingDoor : MonoBehaviour {
	public float maxDoorPosition;
	public float doorSpeed = 0.1f;
	public Transform leftDoor;

	List<GameObject> ObjectsInProximity = new List<GameObject>();

	public bool AdjustSizeAutomatically = false;
	public Vector2 triggerDistance = new Vector2(1.5f, 2.5f);
	void OnValidate() { if (AdjustSizeAutomatically) { GetComponent<BoxCollider2D>().size = triggerDistance; } }

	Vector2 endLeftPosition;
	Vector2 startLeftPosition;

	// TODO: Add authorized people list.
	// Some doors will only allow authorized people to pass through.
	bool isDoorOpen => ObjectsInProximity.Count > 0;

	void Start() {
		startLeftPosition = leftDoor.transform.position;
		endLeftPosition = new Vector2(leftDoor.position.x - maxDoorPosition, leftDoor.position.y);
	}

	protected virtual void Update() {
		if (isDoorOpen) { OpenDoor(); }
		else { CloseDoor(); }
	}

	void OpenDoor() => leftDoor.position = Vector2.Lerp(leftDoor.position, endLeftPosition, doorSpeed);
	void CloseDoor() => leftDoor.position = Vector2.Lerp(leftDoor.position, startLeftPosition, doorSpeed);

	// TODO : Add layer mask if needed.
	// We don't need it currently since the only rigidbodies are the NPCs and the player.
	void OnTriggerEnter2D(Collider2D collision) => ObjectsInProximity.Add(collision.gameObject);
	void OnTriggerExit2D(Collider2D collision) => ObjectsInProximity.Remove(collision.gameObject);
}
