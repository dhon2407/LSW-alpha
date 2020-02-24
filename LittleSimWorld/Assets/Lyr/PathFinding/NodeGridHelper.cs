#if UNITY_EDITOR
namespace PathFinding {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Sirenix.OdinInspector.Editor;
	using UnityEditor;

	[CustomEditor(typeof(NodeGrid2D))]
	public class NodeGridHelper : OdinEditor {

		NodeGrid2D grid => (NodeGrid2D)target;
		Vector2 StartDragPos, EndDragPos;

		Event e; 
		void OnSceneGUI() {
			if (grid.gridData == null || grid.DontDraw) { return; }
			if (!grid.gridData.attemptedLoad) { grid.gridData.Load(); }
			//EdgesHandler(); 
			e = Event.current;
			if (!e.control) { Reset(); return; }
			SetSelected();
			DragManage();
		}

		protected override void OnEnable() {
			base.OnEnable();
			lastTool = Tools.current;
			Tools.current = Tool.None;
			grid.Cache(false);
		}

		protected override void OnDisable() {
			base.OnDisable();
			Tools.current = lastTool;
		}

		Tool lastTool;
		void Reset() {
			grid.selectedNode = null;
			grid.currentlySelected.Clear();
			clickedNode = null;
		}

		void SetSelected() {
			Vector2 pos = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
			grid.selectedNode = grid.NodeFromWorldPoint(pos);
			//Debug.Log("X: " + grid.selectedNode.X + " Y:" + grid.selectedNode.Y);
			SceneView.RepaintAll();
		}

		void DragManage() {

			var controlID = GUIUtility.GetControlID(FocusType.Passive);
			var eventType = e.GetTypeForControl(controlID);
			if (eventType == EventType.MouseUp && e.button == 0) {
				GUIUtility.hotControl = controlID;
				e.Use();
				EndDragPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
				if (grid.currentlySelected.Count == 0) { clickedNode.walkable = !clickedNode.walkable; }
				UpdateGrid();
				grid.currentlySelected.Clear();
			}
			else if (eventType == EventType.MouseDrag) {
				e.Use();
				EndDragPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
				UpdateList();
			}
			else if (eventType == EventType.MouseDown && e.button == 0) {
				GUIUtility.hotControl = 0;
				e.Use();
				grid.currentlySelected.Clear();
				StartDragPos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;
				if (grid.selectedNode != null) { clickedNode = grid.selectedNode; }
			}
		}

		Node clickedNode;
		void UpdateList() {
			grid.currentlySelected.Clear();
			Vector2 dif = EndDragPos - StartDragPos;
			var fX = dif.x / grid.nodeSize;
			var fY = dif.y / grid.nodeSize;

			var absX = Mathf.Max(Mathf.Abs(fX), 0.01f);
			var absY = Mathf.Max(Mathf.Abs(fY), 0.01f);

			for (int i = 0; i < absX; i++) {
				for (int j = 0; j < absY; j++) {
					Vector2 pos = StartDragPos + new Vector2(Mathf.Sign(fX) * i * grid.nodeSize,Mathf.Sign(fY) * j * grid.nodeSize);
					Node node = grid.NodeFromWorldPoint(pos);
					grid.currentlySelected.Add(node);
				}
			}
		}

		void UpdateGrid() {
			foreach (var item in grid.currentlySelected) {
				if (Event.current.shift) { item.walkable = true; }
				else if (Event.current.alt) { item.walkable = false; }
				else { item.walkable = !item.walkable; }
			}
		}


		
		Node[,] ndg => grid.nodeGrid;

		bool inControl;
		Vector2 originalSize = new Vector2(), originalCenter = new Vector2();
		List<Vector3> updatedNodes = new List<Vector3>(), previousPos = new List<Vector3>();

		void EdgesHandler() {
			if (e.control||grid.DontDraw||ndg==null) { inControl = false; return; }
			var controlID = GUIUtility.GetControlID(FocusType.Passive);
			var eventType = e.GetTypeForControl(controlID);

			if (eventType == EventType.MouseUp && e.button == 0 && inControl) {
				inControl = false;
				grid.ResizeGrid();
				//grid.Save(); 
			}
			else if (eventType == EventType.MouseLeaveWindow) { inControl = false; }
			else if (eventType == EventType.MouseDown && e.button == 0) {
				inControl = true;
				originalSize = grid.GridSize;
				originalCenter = grid.Center;
			}

			EditorGUI.BeginChangeCheck();

			if (!inControl) {
				previousPos.Clear();
				updatedNodes.Clear();
				int maxX = ndg.GetLength(0) - 1; int maxY = ndg.GetLength(1) - 1;

				for (int i = 0; i <= 1; i++) {
					for (int j = 0; j <= 1; j++) {
						Vector2 pos = ndg[i * maxX,j * maxY].worldPosition;
						Vector3 updatedNode = Handles.FreeMoveHandle(pos,Quaternion.identity,0.5f,Vector3.one * 0.05f,Handles.CubeHandleCap);
						updatedNodes.Add(updatedNode);
						previousPos.Add(pos);
					}
				}
			}
			else if (inControl) {
				for (int i = 0; i < updatedNodes.Count; i++) {
					updatedNodes[i] = Handles.FreeMoveHandle(updatedNodes[i],Quaternion.identity,0.5f,Vector3.one * 0.05f,Handles.CubeHandleCap);
				}
			}

			if (!EditorGUI.EndChangeCheck()) { return; }
			if (eventType == EventType.MouseDrag && e.button == 0) {
				inControl = true;
				for (int i = 0; i < updatedNodes.Count; i++) {
					if (updatedNodes[i] == previousPos[i]) { continue; }
					Vector2 dir = updatedNodes[i] - previousPos[i];
					Vector2 size = dir;
					if (i == 0) { size *= -1; }
					else if (i == 1) { size.x *= -1; }
					else if (i == 2) { size.y *= -1; }
					else if (i == 3) { size = dir; }
					grid.GridSize = originalSize + size;
					grid.Center = originalCenter + dir / 2;
				}
			}
			
		}

	}
}

#endif