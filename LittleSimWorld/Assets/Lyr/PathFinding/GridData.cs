using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using UnityEngine;
using System.IO;

namespace PathFinding {
	public class GridData : SerializedScriptableObject {
		[System.NonSerialized] public Node[,] nodeGrid;
		[System.NonSerialized] public bool attemptedLoad;

		string filePath => Application.dataPath + "/GridData/" + name + ".GRID_DATA";

		[Button] public void Save() => SaveData();

		[Button] public void Load() => LoadData();

		void SaveData() {
#if UNITY_EDITOR
			if (nodeGrid == null) { Debug.Log("ERROR: Node list empty"); return; }
			if (!File.Exists(filePath)) {
				var file = File.Create(filePath);
				file.Close();
				Debug.Log(filePath + " now exists");
			}

			SaveGrid saveGrid = new SaveGrid();
			saveGrid.grid = nodeGrid;

			DataFormat format = DataFormat.Binary;
			var bytes = SerializationUtility.SerializeValue(saveGrid, format);
			File.WriteAllBytes(filePath, bytes);

			Debug.Log("Saved " + nodeGrid.Length + " nodes.");
#endif
		}

		void LoadData() {
			attemptedLoad = true;

			if (!File.Exists(filePath)) { Debug.LogError($"Failed to Load from {filePath}."); return; }

			DataFormat format = DataFormat.Binary;
			var bytes = File.ReadAllBytes(filePath);
			SaveGrid saveGrid = SerializationUtility.DeserializeValue<SaveGrid>(bytes, format);
			if (saveGrid == null || saveGrid.grid == null) { Debug.LogError("Failed to Load."); return; }
			//Debug.Log("Loaded pathfinding data.");
			nodeGrid = saveGrid.grid;

		}

	}

	[System.Serializable]
	public class SaveGrid {
		[OdinSerialize] public Node[,] grid;
	}

}
