using System.Collections.Generic;
using PathFinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.RandomNPC {
	[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
	public class RandomNPC : SerializedMonoBehaviour {


		public VisualsHelper visualsHelper;

		[System.NonSerialized] public System.Action OnCompleteAction;
		[System.NonSerialized] public Queue<INPCCommand> commandQueue;
		[System.NonSerialized] public Rigidbody2D rb;
		[System.NonSerialized] public Animator anim;
		[System.NonSerialized] public Collider2D col;
		[System.NonSerialized] public List<Node> path;

		INPCCommand currentCommand;
		static List<Collider2D> ignoreColliders => RandomNPCPool.instance.NormallyIgnoredColliders;
		static Collider2D wallCollider;
		static Camera cam;
		static LayerMask carsMask;

		void Awake() {
			anim = GetComponentInChildren<Animator>();
			rb = GetComponent<Rigidbody2D>();
			col = GetComponent<Collider2D>();
			commandQueue = new Queue<INPCCommand>();
			path = new List<Node>(10000);
			col.isTrigger = false;

			if (!wallCollider) { wallCollider = GameObject.Find("Walls").GetComponent<Collider2D>(); }
			if (!cam) { cam = Camera.main; }
			carsMask = LayerMask.GetMask("Cars");
		}

        private void Start()
        {
            GameTime.Clock.Pausing += PauseAnimation;        // Henrique - creating a delegate for controlling the animations on pause and unpausing 
        }

        void Update() {
			if (currentCommand == null) { return; }
			if (currentCommand.interval != CommandInterval.Update) { return; }
			if (!currentCommand.IsFinished) { currentCommand.ExecuteCommand(); }
			else { GetNextCommand(); }
		}

		void FixedUpdate() {
			if (currentCommand == null) { return; }
			if (currentCommand.interval != CommandInterval.FixedUpdate) { return; }

			if (!currentCommand.IsFinished) { currentCommand.ExecuteCommand(); }
			else { GetNextCommand(); }
		}


		public void GetNextCommand() {
			if (commandQueue.Count > 0) {
				currentCommand = commandQueue.Dequeue();
				currentCommand.Initialize();
			}
			else {
				currentCommand = null;
				OnCompleteAction();
				visualsHelper.ResetOrientation();
			}
		}

		void OnCollisionStay2D(Collision2D collision) {
			GameObject hit = collision.gameObject;

			if (hit == GameLibOfMethods.player) { Physics2D.IgnoreCollision(col, wallCollider, false); }
			else if (hit.layer == carsMask.value) {
				if (!isWithinView()) { Physics2D.IgnoreCollision(col, collision.collider, true); }
				Physics2D.IgnoreCollision(col, wallCollider, true);
			}
		}
		void OnCollisionExit2D(Collision2D collision) {
			GameObject hit = collision.gameObject;

			if (hit == GameLibOfMethods.player) { Physics2D.IgnoreCollision(col, wallCollider, true); }
			else if (hit.layer == carsMask.value) {
				Physics2D.IgnoreCollision(col, collision.collider, false);
				Physics2D.IgnoreCollision(col, wallCollider, false);
			}
		}


		bool isWithinView() {
			Vector3 camPos = GameLibOfMethods.player.transform.position;
			float sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;
			float sqrMag = Vector2.SqrMagnitude(transform.position - camPos);
			// We don't want nodes that are within camera view;
			return sqrMag <= 3 * sqrOrthoSize;
		}

		private void PauseAnimation(bool isPausing) // Henrique - creating a delegate for controlling the animations on pause and unpausing
		{
			anim.speed = isPausing ? 0 : 1;
		}
	}
}