namespace PathFinding {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Sirenix.OdinInspector;
	using Sirenix.Serialization;
	using System;

	public class NodeGrid2D : MonoBehaviour {

		#region Inspector

		public Vector2 GridSize;
		[Range(0.1f, 3)] public float nodeSize = 1;
		public Vector2 center;

		public Color gridColor = new Color(0, 0, 1, 0.6f);
		public LayerMask unwalkableMask;
		public string DataName;
		public GridData gridData;

		[Space]
		public bool DontDraw = false;
		public bool DrawNonWired = false;
		#endregion

		#region Initialization			

		[NonSerialized] public int gridX;
		[NonSerialized] public int gridY;
		[NonSerialized] public Vector2 cachedCenter;
		List<Node> walkableNodes;
		void Awake() {
			gridData.Load();
			Cache(true);
			walkableNodes = new List<Node>(nodeGrid.Length);
			foreach (var node in nodeGrid) {
				if (!node.walkable) { continue; }
				walkableNodes.Add(node);
			}
		}

		internal void Cache(bool alsoCacheNeighbors) {

			gridX = Mathf.FloorToInt(GridSize.x / nodeSize) + 1;
			gridY = Mathf.FloorToInt(GridSize.y / nodeSize) + 1;

			if (alsoCacheNeighbors) { CacheNeighbours(); }
			cachedCenter = Center - GridSize / 2;
		}

		void CacheNeighbours() { foreach (var item in nodeGrid) { item.Cache(this); } }

		#endregion

		#region EDITOR STUFF
#if (UNITY_EDITOR)
		[Button]
		public void Save() {
			if (String.IsNullOrEmpty(DataName)) { Debug.LogError("Cannot save empty asset name"); }
			else if (gridData == null || String.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(gridData))) {
				gridData = GridData.CreateInstance<GridData>();
				string path = "Assets/GridData/" + DataName + ".asset";
				UnityEditor.AssetDatabase.CreateAsset(gridData, path);
				gridData.name = DataName;
				FullColliderCheck();

				UnityEditor.EditorUtility.SetDirty(gridData);
				UnityEditor.AssetDatabase.SaveAssets();
			}
			else { gridData.Save(); }
		}

		[Button]
		public void Load() {
			if (gridData != null) { gridData.Load(); }
		}

		[Button]
		public void FullColliderCheck() {
			gridData.nodeGrid = new Node[gridX, gridY];
			gridData.nodeGrid[0, 0] = new Node(false, Center - GridSize / 2, 0, 0);

			for (int x = 0; x < gridX; x++) {
				for (int y = 0; y < gridY; y++) {
					Vector2 pos = (Center - GridSize / 2) + new Vector2(x, y) * nodeSize;
					bool walkable = IsNodeWalkable(pos);
					gridData.nodeGrid[x, y] = new Node(walkable, pos, x, y);
				}
			}
		}

		public void ResizeGrid() {

			if (gridX <= 0 || gridY <= 0) { Debug.Log("BAD INPUT"); return; }
			List<Node> unwalkableNodes = new List<Node>(nodeGrid.Length);
			foreach (var n in nodeGrid) { if (!n.walkable) { unwalkableNodes.Add(n); } }
			gridData.nodeGrid = new Node[gridX, gridY];
			gridData.nodeGrid[0, 0] = new Node(false, Center - GridSize / 2, 0, 0);

			for (int x = 0; x < gridX; x++) {
				for (int y = 0; y < gridY; y++) {
					Vector2 pos = (Center - GridSize / 2) + new Vector2(x * nodeSize, y * nodeSize);
					bool walkable = true; //DONT ADD NEW DATA
					gridData.nodeGrid[x, y] = new Node(walkable, pos, x, y);
				}
			}

			foreach (var n in unwalkableNodes) { NodeFromWorldPoint(n.worldPosition).walkable = false; }
		}

		[HideInInspector, NonSerialized] public List<Node> currentlySelected = new List<Node>();
		[HideInInspector, NonSerialized] public Node selectedNode;
		public void OnDrawGizmosSelected() {
			if (DontDraw || nodeGrid == null) { return; }
			if (Event.current.type != EventType.Repaint) { return; }
			cachedCenter = Center - GridSize / 2;
			Gizmos.DrawWireCube(Center, GridSize);
			Color walkableColor = new Color(1, 1, 1, 0.1f);
			Color unwalkableColor = new Color(1, 0, 0, 0.4f);

			var size = Vector2.one * nodeSize;

			foreach (var item in nodeGrid) {
				if (selectedNode == item) {
					//Gizmos.color = Color.yellow;
					Gizmos.color = new Color(1, 0.9f, 0, 0.3f);
					Gizmos.DrawCube(PosFromNode(item), size);
				}
				if (!item.walkable) {
					Gizmos.color = unwalkableColor;
					if (DrawNonWired) { Gizmos.DrawCube(PosFromNode(item), size); }
					else { Gizmos.DrawWireCube(PosFromNode(item), size); }
				}
				else if (item.isCurrentlyOccupied) {
					Gizmos.color = Color.blue;
					if (DrawNonWired) { Gizmos.DrawCube(PosFromNode(item), size); }
					else { Gizmos.DrawWireCube(PosFromNode(item), size); }
				}
				else {
					Gizmos.color = walkableColor;
					Gizmos.DrawWireCube(PosFromNode(item), size); 
				}
			}

			foreach (var n in currentlySelected) {
				Gizmos.color = new Color(0, 1, 0, 0.3f);
				Gizmos.DrawCube(PosFromNode(n), size);
			}

		}

#endif
		#endregion

		#region Properties
		public Node[,] nodeGrid => gridData.nodeGrid;

		public Vector2 Center {
			get { return (Vector2) transform.position + center; }
			set { center = value - (Vector2) transform.position; }
		}

		#endregion

		#region Node position helpers
		public Vector2 PosFromCoords(int x, int y) => cachedCenter + new Vector2(x * nodeSize, y * nodeSize);
		public Vector2 PosFromNode(Node node) => PosFromCoords(node.X, node.Y);

		public Node NodeFromWorldPoint(Vector2 worldPosition) {

			float percentX = (worldPosition.x - cachedCenter.x) / GridSize.x;
			float percentY = (worldPosition.y - cachedCenter.y) / GridSize.y;

			percentX = Mathf.Clamp01(percentX);
			percentY = Mathf.Clamp01(percentY);

			int x = Mathf.RoundToInt((gridX - 1) * percentX);
			int y = Mathf.RoundToInt((gridY - 1) * percentY);

			return nodeGrid[x, y];
		}
		#endregion

		bool IsNodeWalkable(Vector2 pos) => !Physics2D.OverlapCircle(pos, (nodeSize / 2) - 0.1f, unwalkableMask);
		public bool IsNodeWalkable_Temp(Vector2 pos, LayerMask mask) => !Physics2D.OverlapCircle(pos, (nodeSize / 2) - 0.05f, mask);

		public Node GetRandomWalkable() => walkableNodes[UnityEngine.Random.Range(0, walkableNodes.Count)];

		[Serializable]
		public class TerrainType {
			public LayerMask terrainMask;
			public int terrainPenalty;
		}

	}

}
