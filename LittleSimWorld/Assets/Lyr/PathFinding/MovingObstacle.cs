using Sirenix.OdinInspector;
using UnityEngine;

namespace PathFinding {
	[RequireComponent(typeof(Collider2D))]
	public class MovingObstacle : MonoBehaviour {
		Collider2D col;

		[Tooltip("How often the Obstacle updates its collider's position (0 = each frame)")]
		[SuffixLabel("seconds"), SerializeField]
		float UpdateFrequency = 0;

		void Awake() => col = GetComponent<Collider2D>();

		float currentT;

		void Update() {
			if (UpdateFrequency != 0) {
				currentT += GameTime.Time.deltaTime;
				if (currentT <= UpdateFrequency) { return; }
				currentT = 0;
			}

			NodeGridManager.RegisterUnwalkable(col);
		}

	}
}