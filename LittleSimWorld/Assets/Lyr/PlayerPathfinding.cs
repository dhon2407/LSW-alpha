using System;
using System.Collections.Generic;
using GameTime;
using Objects;
using PlayerStats;
using UnityEngine;

namespace PathFinding {
	[DefaultExecutionOrder(99999)]
	public class PlayerPathfinding : MonoBehaviour {

		Animator anim;
		Rigidbody2D rb;
		SpriteControler spr;
		GameObject player;
		NodeGrid2D grid;
		Collider2D col;
		Transform playerTransform;

		public float Speed = 2;
		float builtUpSpeed = 0;

		void Start() {
			spr = FindObjectOfType<SpriteControler>();

			player = Player.gameObject;
			anim = Player.anim;
			rb = Player.rb;
			grid = NodeGridManager.GetGrid(PathFinding.Resolution.High);
			col = Player.col;
			playerTransform = Player.transform;

			contactFilter = new ContactFilter2D();
			contactFilter.SetLayerMask(LayerMask.GetMask("MouseOverCollider"));
		}

		// TODO: Make current node occupied and use it for other agents 
		// To be added for NPCs to avoid pathing inside the player.
		Node currentNode;
		void MakeCurrentNodeOccupied() => NodeGridManager.SetPosUnwalkable(playerTransform.position, col);

		void LateUpdate() {
			CheckClicks();
			HandleBuiltUpSpeed();
			MakeCurrentNodeOccupied();
            anim.SetBool("Running", Input.GetButton("Run"));
        }

        void CheckClicks() {
			if (Clock.Paused) {
				return;
			}

			bool shouldCancel = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && currentHandle.HasValue;
			if (shouldCancel) { Cancel(); }

            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) return;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            IInteractable interactable = CheckInteractable(pos);

            if (Input.GetMouseButtonDown(0)) {

				if (CustomInputModule.IsPointerBlocked(30)) { return; }

				if (interactable != null) {
					if (currentHandle.HasValue) { Cancel(); }
					currentTarget = interactable;
					Action action = interactable.interactionOptions.UpdateCharacterOrientation;
                    //henrique - added the option to check if the Interface that displays options in a menu
                    var menuOptions = interactable.gameObject.GetComponent<ImenuInteraction>();
                    if (menuOptions != null) action += menuOptions.OpenMenu;
                    else action += interactable.Interact;
                    MoveTo(interactable.interactionOptions.PlayerStandPosition, Speed, action);
				}
			}

			if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0) && CheckInteractable(pos) == null) {
				if (CustomInputModule.IsPointerBlocked(30)) { return; }

				if (currentHandle.HasValue) { Cancel(); }

				if (interactable != null) {
					Action action = interactable.interactionOptions.UpdateCharacterOrientation;
					MoveTo(interactable.interactionOptions.PlayerStandPosition, Speed, action);
				}
				else { MoveTo(pos, Speed, null); }
			}
		}

		void Cancel() {
			if (currentHandle == null) { return; }
			MEC.Timing.KillCoroutines(currentHandle.Value);
			currentHandle = null;
			currentTarget = null;
			anim.SetBool("Walking", false);
			//IgnoreAllCollisionExcept(false);
			col.isTrigger = false;
			PlayerAnimationHelper.ResetPlayer();
		}

		IInteractable CheckInteractable(Vector2 pos) {
			var colliders = new List<Collider2D>();

			Physics2D.OverlapCircle(pos, 0.1f, contactFilter, colliders);

			foreach (Collider2D collider in colliders) {
				Transform targetTransform = collider.transform;

				// Max depth of 3 at the moment
				for (int i = 0; i < 3; i++) {
					if (targetTransform.gameObject.layer == 10 || targetTransform.gameObject.layer == 15) { break; }
					targetTransform = targetTransform.transform.parent;
				}

				IInteractable interactable = targetTransform.GetComponent<IInteractable>();
				if (interactable == null) { continue; }

				return interactable;
			}
			return null;
		}

		void HandleBuiltUpSpeed() {
			if (currentHandle != null) {
				builtUpSpeed = Mathf.MoveTowards(builtUpSpeed, 1, GameTime.Time.deltaTime);
			}
			else if (builtUpSpeed > 0) {
				builtUpSpeed = Mathf.MoveTowards(builtUpSpeed, 0, GameTime.Time.deltaTime);
			}
		}

		#region Movement
		List<Node> path = new List<Node>(1000);
		ContactFilter2D contactFilter = new ContactFilter2D();
		MEC.CoroutineHandle? currentHandle;
		IInteractable currentTarget;
		public void MoveTo(Vector3 pos, float speed, System.Action callback) {
			if (currentHandle.HasValue) { Cancel(); }
			if (GameLibOfMethods.doingSomething) { return; }
			currentHandle = WalkTo(pos, builtUpSpeed, speed, callback).Start(MEC.Segment.FixedUpdate);

		}


		IEnumerator<float> WalkTo(Vector3 Position, float StartingSpeedPercentage, float MaxSpeed, System.Action Callback) {
			Position = new Vector3(Position.x, Position.y, 1);
			PlayerAnimationHelper.StopPlayer();
			//RequestPath.GetPath(rb.position, Position, path, Resolution.High);
			RequestPath.GetPath_Avoidance(rb.position, Position, path, Resolution.High, col);
			//IgnoreAllCollisionExcept(true, 31, 9, 10);
			col.isTrigger = true;

			float minSqrDist = 0;
			Vector3 pathFindingCenter = Vector3.zero;
			if (currentTarget != null) {
				minSqrDist = Mathf.Pow(currentTarget.interactionOptions.InteractionRange, 2);
				pathFindingCenter = currentTarget.gameObject.transform.position + (Vector3) currentTarget.interactionOptions.InteractionCenterOffset;
			}

			yield return 0f;

			if (path.Count != 0) { anim.SetBool("Walking", true); }
            MaxSpeed *= Stats.MoveSpeed;

			float _spd = StartingSpeedPercentage * MaxSpeed;
			int index = 0;

			while (this) {
				if (index >= path.Count) { break; }

				if (!CanWalkAt(path[index])) {
					yield return 0f;
					//RequestPath.GetPath(rb.position, Position, path, Resolution.High);
					RequestPath.GetPath_Avoidance(rb.position, Position, path, Resolution.High, col);
					index = 0;
					continue;
				}

				Vector3 currentPos = rb.position;
				Vector3 targetPos = grid.PosFromNode(path[index]);

				var offset = new Vector3(targetPos.x - currentPos.x, targetPos.y - currentPos.y, 1);
				if (_spd != MaxSpeed) { _spd = Mathf.MoveTowards(_spd, MaxSpeed, 10 * MaxSpeed * GameTime.Time.fixedDeltaTime); }
				float currentSpeed = _spd * GameTime.Time.fixedDeltaTime;

				var posAfterMovement = Vector3.MoveTowards(currentPos, targetPos, currentSpeed);

				// Update the index and increase the movement point without losing a frame of movement.
				if (posAfterMovement == targetPos) {
					index++;

					if (index >= path.Count) { }
					else {
						targetPos = new Vector3(grid.PosFromNode(path[index]).x, grid.PosFromNode(path[index]).y, 1);
						offset = new Vector3(targetPos.x - currentPos.x, targetPos.y - currentPos.y, 1);

						posAfterMovement = Vector3.MoveTowards(currentPos, targetPos, currentSpeed);
					}
				}

				/// Check if we've reached the <see cref="Objects.Functionality.InteractionOptions.InteractionRange"/>.
				if (currentTarget != null) {
					float sqrDistanceToTarget = Vector2.SqrMagnitude(posAfterMovement - pathFindingCenter);
					if (sqrDistanceToTarget <= minSqrDist) { break; }
				}

				rb.MovePosition(posAfterMovement);

				if (index >= path.Count) { break; }

				CalculateFacing(offset);

				yield return 0f;
			}

			if (!this) { yield break; }

			anim.SetBool("Walking", false);
			yield return 0f;

			col.isTrigger = false;
			PlayerAnimationHelper.ResetPlayer();
			currentHandle = null;
			currentTarget = null;

			if (Callback != null) { Callback(); }
		}


		void CalculateFacing(Vector2 offset) {
			//bool isYBigger = Mathf.Abs(offset.x) <= 0.01f;
			bool isYBigger = Mathf.Abs(offset.x) < Mathf.Abs(offset.y);

			if (isYBigger) {
				if (offset.y > 0) { spr.FaceUP(); }
				else if (offset.y < 0) { spr.FaceDOWN(); }

				if (offset.y != 0) { anim.SetFloat("Vertical", Mathf.Sign(offset.y)); }
				else { anim.SetFloat("Vertical", 0); }

				anim.SetFloat("Horizontal", 0);
			}
			else {
				if (offset.x > 0) { spr.FaceRIGHT(); }
				else if (offset.x < 0) { spr.FaceLEFT(); }

				if (offset.x != 0) { anim.SetFloat("Horizontal", Mathf.Sign(offset.x)); }
				else { anim.SetFloat("Horizontal", 0); }

				anim.SetFloat("Vertical", 0);
			}
		}
		#endregion

		bool CanWalkAt(Node node) => node.isCurrentlyOccupied == null || node.isCurrentlyOccupied == col;
	}
}