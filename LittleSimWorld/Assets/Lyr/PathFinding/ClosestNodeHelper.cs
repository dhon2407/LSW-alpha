
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding {
	public static class NodeHelper {
		static List<Node> helperList = new List<Node>(80);
		static NodeGrid2D grid;
		static Node[,] nodeGrid;

		const int maxTries = 10;
		const int maxRadius = 5;
		static int X, Y, maxX, maxY;


		static Func<int, int> decrease = Decrease;
		static Func<int, int> increase = Increase;
		static Func<int, int> stay = Stay;

		public static Node ClosestWalkable(Node closestTo, Resolution resolution) {

			grid = NodeGridManager.GetGrid(resolution);
			nodeGrid = grid.nodeGrid;

			// First, check if any of the neighbors are walkable
			BaseNode[] neighbors = closestTo.neighbours.directionNeighbours;

			X = closestTo.X;
			Y = closestTo.Y;

			maxX = nodeGrid.GetLength(0) - 1;
			maxY = nodeGrid.GetLength(1) - 1;

			helperList.Clear();


			// Find Closest Left
			helperList.Add(ClosestToOrigin(decrease, stay));

			// Find Closest Right
			helperList.Add(ClosestToOrigin(increase, stay));

			// Find Closest Top
			helperList.Add(ClosestToOrigin(stay, increase));

			// Find Closest Bot
			helperList.Add(ClosestToOrigin(stay, decrease));

			// Find Closest Diags
			helperList.Add(ClosestToOrigin(decrease, decrease));
			helperList.Add(ClosestToOrigin(decrease, increase));
			helperList.Add(ClosestToOrigin(increase, decrease));
			helperList.Add(ClosestToOrigin(increase, increase));

			// Find Most efficient Node
			int minDist = int.MaxValue;
			Node closestNode = null;

			foreach (Node n in helperList) {
				if (n == null) { continue; }

				int dist = heuristic(n, closestTo);
				if (dist >= minDist) { continue; }

				minDist = dist;
				closestNode = n;
			}

			if (closestNode == null) {
				Debug.LogError("Couldn't find valid node on " + grid.PosFromNode(closestTo));
				return closestTo;
			}
			else { return closestNode; }

		}

		#region Helper Methods
		static Node ClosestToOrigin(Func<int, int> opX, Func<int, int> opY) {
			int x = X;
			int y = Y;

			for (int i = 0; i < maxTries; i++) {
				x = opX(x);
				y = opY(y);

				if (y < 0 || y > maxY) { break; }
				if (x < 0 || x > maxX) { break; }

				Node n = nodeGrid[x, y];
				if (n.walkable) { return n; }
			}

			return null;
		}

		static int Increase(int a) => a + 1;
		static int Decrease(int a) => a - 1;
		static int Stay(int a) => a;

		static int heuristic(Node A, Node B) {
			int distX = (A.X > B.X ? A.X - B.X : B.X - A.X);
			int distY = (A.Y > B.Y ? A.Y - B.Y : B.Y - A.Y);

			return distX + distY;
		}
		#endregion

		//public static TSource First<TSource>(this IList<TSource> source, Func<TSource, bool> predicate) {
		//	for (int i = 0; i < source.Count; i++) {
		//		if (predicate(source[i])) { return source[i]; }
		//	}
		//	return default(TSource);
		//}

	}
}