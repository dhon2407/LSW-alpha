namespace PathFinding {
	using System.Collections;
	using System.Collections.Generic;
	using PathFinding;
	using Sirenix.OdinInspector;
	using UnityEngine;
    using UnityEngine.SceneManagement;

    public enum Resolution { Low, Medium, High }

	[DefaultExecutionOrder(999999)]
	public class NodeGridManager : SerializedMonoBehaviour {


		public NodeGrid2D High_Resolution_Grid;
		public NodeGrid2D Medium_Resolution_Grid;
		public NodeGrid2D Medium_Resolution_Grid_Spawn;

		[Space] public LayerMask mask;
		public int MinCheckAmount = 10;

		[ShowInInspector] public int Count => tempUnwalkableNodes_HIGH.Count + tempUnwalkableNodes_MEDIUM.Count;

		HashSet<Node> tempUnwalkableNodes_HIGH = new HashSet<Node>();
		HashSet<Node> tempUnwalkableNodes_MEDIUM = new HashSet<Node>();
        HashSet<Node> tempUnwalkableNodes_LOW = new HashSet<Node>();
        List<Node> tempNodeList = new List<Node>(1000);

		static Collider2D playerCol;
		public static NodeGridManager instance;
		void Awake() {
			instance = this;
			playerCol = GameLibOfMethods.player.GetComponent<Collider2D>();
		}


		#region Static functionality
		public static NodeGrid2D GetGrid(Resolution resolution) {
			if (resolution == Resolution.Medium) { return instance.Medium_Resolution_Grid; }
			else if (resolution == Resolution.High) { return instance.High_Resolution_Grid; }
            else if (resolution == Resolution.Low) { return instance.Medium_Resolution_Grid_Spawn; }
            else { throw new UnityException("GetGrid() was called for not supported resolution."); }
		}

		public static void SetPosUnwalkable(Vector2 pos, Collider2D col) {
			AddToList(instance.tempUnwalkableNodes_HIGH, GetGrid(Resolution.High));
			AddToList(instance.tempUnwalkableNodes_MEDIUM, GetGrid(Resolution.Medium));
            AddToList(instance.tempUnwalkableNodes_LOW, GetGrid(Resolution.Low));

            void AddToList(HashSet<Node> list, NodeGrid2D grid) {
				var targetNode = grid.NodeFromWorldPoint(pos);
				if (!targetNode.isCurrentlyOccupied) { targetNode.isCurrentlyOccupied = col; }
				list.Add(targetNode);
			}

		}
		public static void RegisterUnwalkable(Collider2D col) {
			var bounds = col.bounds;
			Vector2 center = bounds.center;
			Vector2 extents = bounds.extents;

			var nodeSize = instance.High_Resolution_Grid.nodeSize;

			int iterationsX = Mathf.RoundToInt(extents.x / nodeSize);
			int iterationsY = Mathf.RoundToInt(extents.y / nodeSize);

			for (int x = -iterationsX; x <= iterationsX; x++) {
				for (int y = -iterationsY; y <= iterationsY; y++) {
					var dif = new Vector2(x, y) * nodeSize;
					SetPosUnwalkable(center + dif, col);
				}
			}
		}
		#endregion

		void LateUpdate() => CheckUnwalkables();
		void CheckUnwalkables() {
			Physics2D.autoSyncTransforms = false;
			Physics2D.SyncTransforms();
			CheckForChanges(tempUnwalkableNodes_HIGH, High_Resolution_Grid);
			CheckForChanges(tempUnwalkableNodes_MEDIUM, Medium_Resolution_Grid);
			CheckForChanges(tempUnwalkableNodes_LOW, Medium_Resolution_Grid_Spawn);
			Physics2D.autoSyncTransforms = true;

			void CheckForChanges(HashSet<Node> hashSet, NodeGrid2D grid) {
				tempNodeList.Clear();

				// Turns out to be the most performant way.
				// compared with the alternative : hashSet.RemoveWhere(...)
				foreach (var node in hashSet) {
					var pos = grid.PosFromCoords(node.X, node.Y);
					//if (!grid.IsNodeWalkable_Temp(pos, mask)) { continue; }
					if (node.isCurrentlyOccupied && node.isCurrentlyOccupied.OverlapPoint(pos)) { continue; }
					tempNodeList.Add(node);
				}
				foreach (var node in tempNodeList) {
					node.isCurrentlyOccupied = null;
					hashSet.Remove(node);
				}
			}

		}

        //Gizmos -- enable for Debug
        //void OnDrawGizmosSelected()
        //{

        //    DrawFromList(tempUnwalkableNodes_LOW, GetGrid(Resolution.Low));

        //    void DrawFromList(HashSet<Node> list, NodeGrid2D grid)
        //    {
        //        var size = grid.nodeSize * Vector2.one;
        //        Gizmos.color = Color.blue;
        //        foreach (var item in list)
        //        {
        //            Gizmos.DrawCube(grid.PosFromNode(item), size);
        //        }
        //    }
        //}

    }
}