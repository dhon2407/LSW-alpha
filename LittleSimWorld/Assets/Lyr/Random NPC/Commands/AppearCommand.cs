using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;
using random = System.Random;

namespace Characters.RandomNPC {
	public class AppearCommand : INPCCommand {

		RandomNPC parent;

		static Camera cam;
		const int maxAttemptsForNewLocation = 20;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.Update;

		public AppearCommand(RandomNPC parent) {
			this.parent = parent;
			cam = Camera.main;
		}

		public void Initialize() {
			IsFinished = true;
			SpawnOutOfView();
		}

		void SpawnOutOfView() {
			var camPos = (Vector2) GameLibOfMethods.player.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			var grid = NodeGridManager.GetGrid(PathFinding.Resolution.Low);

			int currentAttempt = 0;
            while (currentAttempt <= maxAttemptsForNewLocation)
            {
                currentAttempt++;
                //var rndNode = grid.GetRandomWalkable();
                //Test
                Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                random rnd = new random();

                float spawnX = rnd.Next(0, 2);
                float spawnY = rnd.Next(0, 2);

                spawnX = AdjustSpawnPoints(spawnX);
                spawnY = AdjustSpawnPoints(spawnY);

                Vector3 spawnPoint = camera.ViewportToWorldPoint(new Vector3(spawnX, spawnY, camera.nearClipPlane));
                Vector2 newLoc = new Vector2(spawnPoint.x, spawnPoint.y);

                var rndNode = grid.NodeFromWorldPoint(newLoc);
                
                //Test End

                // We don't want nodes that are occupied
                if (rndNode.isCurrentlyOccupied != null) { continue; }
                if (!rndNode.walkable) { continue; }

                var sqrMag = Vector2.SqrMagnitude(newLoc - camPos);


                // We don't want nodes that are within camera view;
                if (sqrMag <= 2 * sqrOrthoSize) { continue; }

                parent.transform.position = newLoc;
                return;
            }

            // We reach here if the max attempts limit has been exceeded
            // TODO: Make sure it never happens.. somehow..
            Debug.Log("Max Attempts limit on spawning NPC outside the camera's frustum has been exceeded.");

            // Safety for not appearing on the first frame
            // .. has to happen since the physics won't update until FixedUpdate, and we do this in Update

            parent.transform.position = Vector3.one * 1000;
        }

        private float AdjustSpawnPoints(float spawnP)
        {
            if (spawnP == 0)
                return spawnP -= 0.1f;
            else return spawnP += 0.1f;
        }


        public void ExecuteCommand() { }

	}

	public class DisappearCommand : INPCCommand {

		RandomNPC parent;

		static Camera cam;

		public bool IsFinished { get; set; }
		public CommandInterval interval => CommandInterval.Update;

		public DisappearCommand(RandomNPC parent) {
			this.parent = parent;
			cam = Camera.main;
		}

		// TODO: Change or implement this targetAlpha variable 
		public void Initialize() {
			IsFinished = true;
			DisappearSafely();
		}


		void DisappearSafely() {
			var camPos = cam.transform.position;
			var pos = parent.transform.position;
			var sqrOrthoSize = cam.orthographicSize * cam.orthographicSize;

			var sqrMag = Vector2.SqrMagnitude(pos - camPos);
			if (sqrMag >= 3 * sqrOrthoSize) { return; }

			var randomNode = NodeGridManager.GetGrid(PathFinding.Resolution.Medium).GetRandomWalkable();
			var newFadePos = NodeGridManager.GetGrid(PathFinding.Resolution.Medium).PosFromNode(randomNode);

			parent.commandQueue.Enqueue(new HangAroundCommand(parent, Random.Range(1, 5f)));
			parent.commandQueue.Enqueue(new MoveToCommand(parent, newFadePos));
			parent.commandQueue.Enqueue(new DisappearCommand(parent));
		}

		// TODO: Make this applicable for fading away, like for going to work or whatnot.
		public void ExecuteCommand() {
			//t += Speed * Time.deltaTime;
			// parent.SetAlpha(
		}

	}



}