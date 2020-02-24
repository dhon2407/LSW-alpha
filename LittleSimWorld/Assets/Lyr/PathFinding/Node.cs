namespace PathFinding {
	using UnityEngine;
	using System;

	[Serializable]
	public class Node {
		public bool walkable;
		public readonly Vector2 worldPosition;
		public readonly int X, Y;

		//public List<NodeBase> neighbours = new List<NodeBase>(8);
		[NonSerialized] public Neighbours neighbours;
		[NonSerialized] public int gCost, hCost;
		[NonSerialized] public int heapIndex;
		[NonSerialized] public BaseNode parent;
		[NonSerialized] public Collider2D isCurrentlyOccupied;

		public Node(bool _walkable,Vector2 _worldPos,int _gridX,int _gridY) {
			walkable = _walkable;
			worldPosition = _worldPos;
			X = _gridX; Y = _gridY;
			//neighbours = new Neighbours(X,Y,NodeGrid2D.instance);
		}


		public void Cache(NodeGrid2D grid) {
			neighbours = new Neighbours();
			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					// We don't want to cache the node itself as neighbor.
					if (x == 0 && y == 0) { continue; }

					int checkX = X + x;
					int checkY = Y + y;

					bool canAdd = (checkX >= 0 && checkX < grid.gridX) && (checkY >= 0 && checkY < grid.gridY);

					if (!canAdd) { continue; }

					bool isDiagonal = (x != 0 && y != 0);

					// Comment out this line to enable diagonals
						// if (isDiagonal) { continue; }

					neighbours.Add(new BaseNode(checkX, checkY), isDiagonal);
				}
			}
		}

		public bool IsMoreEfficient(Node obj) {
			var CostA = gCost + hCost;
			var CostB = obj.gCost + obj.hCost;

			if (CostA == CostB) { return hCost < obj.hCost; }
			return CostA <= CostB;
		}

		//public override bool Equals(object obj) => obj.GetHashCode() == GetHashCode();
		//// Hash Code optimization
		//public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() << 2;
	}

	// Helpers
	public struct BaseNode {
		public readonly int x, y;
		public BaseNode(int X,int Y) { x = X; y = Y; }

	}

	public class Neighbours {
		public BaseNode[] directionNeighbours = new BaseNode[4];
		public BaseNode[] diagonalNeighbours = new BaseNode[4];

		public int DirectionCount;
		public int DiagonalCount;

		public void Clear() => DirectionCount = DiagonalCount = 0;
		public void Add(BaseNode node,bool diagonal) {
			if (!diagonal) {
				directionNeighbours[DirectionCount] = node;
				DirectionCount++;
			}
			else {
				diagonalNeighbours[DiagonalCount] = node;
				DiagonalCount++;
			}
		}
	}
}
