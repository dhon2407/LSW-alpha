using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PathFinding {
	public static class RequestPath {

		#region INITIALIZATION

		static OpenHeap openSet; static ClosedHeap closedSet;
		static int minCheckAmount = NodeGridManager.instance.MinCheckAmount;

		static RequestPath() {
			// Magic numbers.. no actual cost in memory / performance tho (400kb total)
			// NOTE: It's currently at about max node size (~19.442) of the highest resolution grid

			// Increase to a max node size if we choose to upscale the resolution of the grid and implement width-based pathfinding

			// Should be (int) (gridX * gridY / nodeSize) if we want to get the actual numbers
			openSet = new OpenHeap(20000);
			closedSet = new ClosedHeap(20000);
		}

		#endregion

		public static void GetPath(Vector2 start, Vector2 target, List<Node> pp, Resolution resolution) {

			#region INIT
			var grid = NodeGridManager.GetGrid(resolution);
			var nodeGrid = grid.nodeGrid;

			Node startNode = grid.NodeFromWorldPoint(start);
			Node targetNode = grid.NodeFromWorldPoint(target);

			if (!startNode.walkable) { startNode = NodeHelper.ClosestWalkable(startNode,resolution); }
			if (!targetNode.walkable) { targetNode = NodeHelper.ClosestWalkable(targetNode, resolution); }

			Node node, nbr;
			openSet.Clear();
			closedSet.Clear();
			openSet.Add(startNode);
			startNode.hCost = startNode.gCost = 0;
			#endregion

			while (openSet.Count > 0) {
				node = openSet.RemoveFirst();
				closedSet.Add(node);

				if (node == targetNode) {
					pp.Clear();
					Node currentNode = targetNode;
					while (currentNode != startNode) {
						pp.Add(currentNode);
						currentNode = nodeGrid[currentNode.parent.x, currentNode.parent.y];
					}
					pp.Reverse();
					return;
				}

				bool skipD = false;

				var nbsA = node.neighbours.directionNeighbours;
				int scanA = node.neighbours.DirectionCount;
				for (int i = 0; i < scanA; i++) {
					nbr = nodeGrid[nbsA[i].x, nbsA[i].y];
					if (!nbr.walkable) { skipD = true; continue; }
					if (closedSet.Contains(nbr)) { continue; }

					int cost = node.gCost + heuristic(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
					}
				}
				if (skipD) { continue; }

				var nbsB = node.neighbours.diagonalNeighbours;
				int scanB = node.neighbours.DiagonalCount;

				for (int i = 0; i < scanB; i++) {
					nbr = nodeGrid[nbsB[i].x, nbsB[i].y];
					if (!nbr.walkable) { continue; }
					if (closedSet.Contains(nbr)) { continue; }

					for (int j = 0; j < nbr.neighbours.DiagonalCount; j++) {
						var nbr_D = nbr.neighbours.diagonalNeighbours[j];
						var dNode = nodeGrid[nbr_D.x, nbr_D.y];
						if (!dNode.walkable) { break; }

					}

					int cost = node.gCost + heuristic(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
					}
				}
			}

			// If the code escapes the while loop, it means no valid path could be found.
			#if UNITY_EDITOR
			Debug.LogWarning($"Path not found (Start: {start} -- Target: {target})");
			#endif
		}

		public static void GetPath_Avoidance(Vector2 start, Vector2 target, List<Node> pp, Resolution resolution, Collider2D col) {

			#region INIT
			var grid = NodeGridManager.GetGrid(resolution);
			var nodeGrid = grid.nodeGrid;

			Node startNode = grid.NodeFromWorldPoint(start);
			Node targetNode = grid.NodeFromWorldPoint(target);

			if (!startNode.walkable) { startNode = NodeHelper.ClosestWalkable(startNode, resolution); }
			if (!targetNode.walkable) { targetNode = NodeHelper.ClosestWalkable(targetNode, resolution); }

			Node node, nbr;
			openSet.Clear();
			closedSet.Clear();
			openSet.Add(startNode);
			startNode.hCost = startNode.gCost = 0;
			#endregion

			int checkAmount = 0;

			while (openSet.Count > 0) {
				node = openSet.RemoveFirst();
				closedSet.Add(node);

				if (node == targetNode) {
					pp.Clear();
					Node currentNode = targetNode;
					while (currentNode != startNode) {
						pp.Add(currentNode);
						currentNode = nodeGrid[currentNode.parent.x, currentNode.parent.y];
					}
					pp.Reverse();
					return;
				}

				bool skipD = false;

				var nbsA = node.neighbours.directionNeighbours;
				int scanA = node.neighbours.DirectionCount;
				for (int i = 0; i < scanA; i++) {
					nbr = nodeGrid[nbsA[i].x, nbsA[i].y];
					if (!nbr.walkable) { skipD = true; continue; }
					if (checkAmount < minCheckAmount && nbr.isCurrentlyOccupied && nbr.isCurrentlyOccupied != col) { skipD = true; continue; }
					if (closedSet.Contains(nbr)) { continue; }

					int cost = node.gCost + heuristic(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
						checkAmount++;
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
					}
				}


				if (skipD) { continue; }

				var nbsB = node.neighbours.diagonalNeighbours;
				int scanB = node.neighbours.DiagonalCount;

				for (int i = 0; i < scanB; i++) {
					nbr = nodeGrid[nbsB[i].x, nbsB[i].y];
					if (checkAmount < minCheckAmount && nbr.isCurrentlyOccupied && nbr.isCurrentlyOccupied != col) { continue; }
					if (!nbr.walkable) { continue; }
					if (closedSet.Contains(nbr)) { continue; }

					for (int j = 0; j < nbr.neighbours.DiagonalCount; j++) {
						var nbr_D = nbr.neighbours.diagonalNeighbours[j];
						var dNode = nodeGrid[nbr_D.x, nbr_D.y];
						if (!dNode.walkable) { break; }
						if (checkAmount < minCheckAmount && dNode.isCurrentlyOccupied && dNode.isCurrentlyOccupied != col) { break; }
					}

					int cost = node.gCost + heuristic(node, nbr);
					if (!openSet.Contains(nbr)) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
						checkAmount++;
						openSet.Add(nbr);
					}
					else if (cost < nbr.gCost) {
						nbr.gCost = cost;
						nbr.hCost = heuristic(nbr, targetNode);
						nbr.parent = new BaseNode(node.X, node.Y);
					}
				}
			}

			pp.Clear();
			// If the code escapes the while loop, it means no valid path could be found.
			#if UNITY_EDITOR
			//Debug.LogWarning($"Path not found (Start: {start} -- Target: {target})");
			#endif
		}

		static int heuristic(Node A, Node B) {
			// Performant Mathf.Abs 
			int distX = (A.X > B.X ? A.X - B.X : B.X - A.X);
			int distY = (A.Y > B.Y ? A.Y - B.Y : B.Y - A.Y);

			// 14 == 10 * ~sqrt(2) to calculate the diagonals
			if (distX > distY) { return 14 * distY + 10 * (distX - distY); }
			else { return 14 * distX + 10 * (distY - distX); }
		}

	}
}
