using UnityEngine;

public class OccluderMover : MonoBehaviour {
	public Transform target;
	public float targetZ = 1;

	void LateUpdate() {
		var pos = target.position;
		pos.z = targetZ;
		transform.position = pos;
		transform.rotation = target.rotation;
	}
}
