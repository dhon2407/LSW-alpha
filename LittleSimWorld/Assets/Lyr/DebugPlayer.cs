using UnityEngine;

public class DebugPlayer : MonoBehaviour {
	public Animator anim;
	public float walkingSpeed;
	Rigidbody2D rb;

	void Awake() => rb = GetComponent<Rigidbody2D>();

	void FixedUpdate() {
		float speedMulti = walkingSpeed;
		Vector2 temp = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		if (temp.sqrMagnitude >= 1) { temp.Normalize(); }
		temp *= speedMulti;

		rb.velocity = temp;
	}
}
