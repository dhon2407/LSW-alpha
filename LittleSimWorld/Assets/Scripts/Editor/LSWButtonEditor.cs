using UI.Buttons;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LSWButton))]
public class LSWButtonEditor : Editor
{
    [MenuItem("GameObject/UI/LSW Button", false,1)]
    static void CreateCanvas(MenuCommand menuCommand){
        GameObject go = new GameObject("LSW Button");
        go.AddComponent<RectTransform>();
        go.AddComponent<LSWButton>();
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
