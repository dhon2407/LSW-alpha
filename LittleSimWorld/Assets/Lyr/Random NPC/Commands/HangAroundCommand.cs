using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Characters.RandomNPC {
	public class HangAroundCommand : INPCCommand {

		RandomNPC parent;
		Vector2 Origin;
		float time;

		List<Node> path;

		Rigidbody2D rb => parent.rb;
		Animator anim => parent.anim;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.FixedUpdate;

		public HangAroundCommand(RandomNPC parent, float time) {
			this.parent = parent;
			this.time = time;
		}

		public void Initialize() {
			path = new List<Node>(100);
			this.Origin = rb.position;
			parent.col.isTrigger = false;
		}

		float t = 0;

		public void ExecuteCommand() {
			t += Time.fixedDeltaTime * GameTime.Clock.TimeMultiplier;
			rb.velocity = Vector2.zero;

			if (t >= time) {
				IsFinished = true;
				//parent.GetComponent<Collider2D>().isTrigger = true;
			}
		}

	}
}