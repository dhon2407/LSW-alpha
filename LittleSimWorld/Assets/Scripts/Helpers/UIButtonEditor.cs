#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;

namespace LSW.Helpers {
    [CustomEditor(typeof(ButtonHold))]
    public class UIButtonEditor : ButtonEditor {
        SerializedProperty _holdTriggerTime;
        SerializedProperty _triggerRate;

        bool isExpanded;

        protected override void OnEnable() {
            _holdTriggerTime = serializedObject.FindProperty("holdTriggerTime");
            _triggerRate = serializedObject.FindProperty("triggerRate");
            base.OnEnable();
        }
        public override void OnInspectorGUI() {
            EditorGUILayout.LabelField("Hold Button properties", EditorStyles.boldLabel);
            _holdTriggerTime.floatValue = EditorGUILayout.FloatField("Hold trigger time", _holdTriggerTime.floatValue);
            _triggerRate.floatValue = EditorGUILayout.FloatField("Trigger rate", _triggerRate.floatValue);

            EditorGUILayout.Space();

            isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isExpanded, "Base Button Properties");
            if (isExpanded) { base.OnInspectorGUI(); }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
#endif