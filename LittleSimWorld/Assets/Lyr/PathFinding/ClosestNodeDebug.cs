using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace PathFinding {
	public class ClosestNodeDebug : MonoBehaviour {

		public Resolution resolution;

		NodeGrid2D grid;

		void Start() => grid = NodeGridManager.GetGrid(resolution);

		void OnDrawGizmosSelected() {
			if (grid== null) { return; }
			if (grid.DontDraw) { return; }
			if (Camera.main == null) { return; }

			var mousePos = Input.mousePosition;
			var pos = Camera.main.ScreenToWorldPoint(mousePos);
			var node = grid.NodeFromWorldPoint(pos);

			Gizmos.color = Color.green;
			Gizmos.DrawCube(grid.PosFromNode(node), Vector3.one * 0.5f);
			if (node.walkable) {
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(grid.PosFromNode(node), Vector3.one * 0.45f);
			}
			else {
				var closestNode = NodeHelper.ClosestWalkable(node, resolution);
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(grid.PosFromNode(closestNode), Vector3.one * 0.45f);
			}
		}
	}
}