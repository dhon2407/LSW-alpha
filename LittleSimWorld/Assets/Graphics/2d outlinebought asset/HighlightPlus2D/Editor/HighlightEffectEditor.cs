﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HighlightPlus2D {
	[CustomEditor (typeof(HighlightEffect2D))]
	[CanEditMultipleObjects]
	public class HighlightEffectEditor : Editor {

		SerializedProperty previewInEditor, ignore, highlighted, occluder;
		SerializedProperty pixelSnap, alphaCutOff, pivotPos, polygonPacking, autoSize, center, scale, aspectRatio;
		SerializedProperty overlay, overlayColor, overlayAnimationSpeed, overlayMinIntensity, overlayBlending, overlayRenderQueue;
		SerializedProperty outline, outlineColor, outlineWidth, outlineQuality, outlineSmooth, outlineOnTop, outlineExclusive;
		SerializedProperty glow, glowWidth, glowDithering, glowMagicNumber1, glowMagicNumber2, glowAnimationSpeed, glowSmooth, glowQuality, glowOnTop, glowPasses;
		SerializedProperty zoomScale;
		SerializedProperty shadowIntensity, shadowColor, shadowOffset, shadow3D;
		SerializedProperty seeThrough, seeThroughIntensity, seeThroughTintAlpha, seeThroughTintColor;

		void OnEnable () {
			ignore = serializedObject.FindProperty ("ignore");
			previewInEditor = serializedObject.FindProperty ("previewInEditor");
			polygonPacking = serializedObject.FindProperty ("polygonPacking");
			highlighted = serializedObject.FindProperty ("_highlighted");
			occluder = serializedObject.FindProperty ("occluder");
			overlay = serializedObject.FindProperty ("overlay");
			overlayColor = serializedObject.FindProperty ("overlayColor");
			overlayAnimationSpeed = serializedObject.FindProperty ("overlayAnimationSpeed");
			overlayMinIntensity = serializedObject.FindProperty ("overlayMinIntensity");
			overlayBlending = serializedObject.FindProperty ("overlayBlending");
			overlayRenderQueue = serializedObject.FindProperty ("overlayRenderQueue");
			outline = serializedObject.FindProperty ("outline");
			outlineColor = serializedObject.FindProperty ("outlineColor");
			outlineWidth = serializedObject.FindProperty ("outlineWidth");
			outlineSmooth = serializedObject.FindProperty ("outlineSmooth");
			outlineQuality = serializedObject.FindProperty ("outlineQuality");
			outlineOnTop = serializedObject.FindProperty ("outlineOnTop");
            outlineExclusive = serializedObject.FindProperty("outlineExclusive");
            glow = serializedObject.FindProperty ("glow");
			glowWidth = serializedObject.FindProperty ("glowWidth");
			glowAnimationSpeed = serializedObject.FindProperty ("glowAnimationSpeed");
			glowDithering = serializedObject.FindProperty ("glowDithering");
			glowMagicNumber1 = serializedObject.FindProperty ("glowMagicNumber1");
			glowMagicNumber2 = serializedObject.FindProperty ("glowMagicNumber2");
			glowSmooth = serializedObject.FindProperty ("glowSmooth");
			glowQuality = serializedObject.FindProperty ("glowQuality");
			glowOnTop = serializedObject.FindProperty ("glowOnTop");
			glowPasses = serializedObject.FindProperty ("glowPasses");
			seeThrough = serializedObject.FindProperty ("seeThrough");
			seeThroughIntensity = serializedObject.FindProperty ("seeThroughIntensity");
			seeThroughTintAlpha = serializedObject.FindProperty ("seeThroughTintAlpha");
			seeThroughTintColor = serializedObject.FindProperty ("seeThroughTintColor");
			pixelSnap = serializedObject.FindProperty ("pixelSnap");
			alphaCutOff = serializedObject.FindProperty ("alphaCutOff");
			pivotPos = serializedObject.FindProperty ("pivotPos");
			autoSize = serializedObject.FindProperty ("autoSize");
			center = serializedObject.FindProperty ("center");
			scale = serializedObject.FindProperty ("scale");
			aspectRatio = serializedObject.FindProperty ("aspectRatio");
			zoomScale = serializedObject.FindProperty ("zoomScale");
			shadowIntensity = serializedObject.FindProperty ("shadowIntensity");
			shadowColor = serializedObject.FindProperty ("shadowColor");
			shadowOffset = serializedObject.FindProperty ("shadowOffset");
			shadow3D = serializedObject.FindProperty ("shadow3D");
		}

		public override void OnInspectorGUI () {
			HighlightEffect2D thisEffect = (HighlightEffect2D)target;
			bool isManager = thisEffect.GetComponent<HighlightManager2D> () != null;
			EditorGUILayout.Separator ();
			serializedObject.Update ();
			if (isManager) {
				EditorGUILayout.HelpBox ("These are default settings for highlighted objects. If the highlighted object already has a Highlight Effect component, those properties will be used.", MessageType.Info);
			} else {
				EditorGUILayout.PropertyField (occluder, new GUIContent ("Occluder", "Add depth compatibility to this object making it an occluder to other sprites so see-through works properly. Only needed in this object does not write to z-buffer."));
				if (occluder.boolValue) {
					EditorGUILayout.HelpBox ("Make sure this sprite is in front of other sprites in the Z axis (adjust transform's position Z value).", MessageType.Info);
				} else {
					EditorGUILayout.PropertyField (previewInEditor);
				}
			}
			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("Sprite Options", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (pixelSnap);
			EditorGUILayout.PropertyField (alphaCutOff);
			EditorGUILayout.PropertyField (polygonPacking);
			if (!polygonPacking.boolValue) {
				EditorGUILayout.LabelField ("Sprite Pivot", pivotPos.vector2Value.ToString ("F4"));
				EditorGUILayout.PropertyField (autoSize);
				GUI.enabled = !autoSize.boolValue;
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField (scale);
				EditorGUILayout.PropertyField (aspectRatio);
				EditorGUILayout.PropertyField (center);
				EditorGUI.indentLevel--;
				GUI.enabled = true;
			}
			if (!occluder.boolValue) {
				EditorGUILayout.Separator ();
				EditorGUILayout.LabelField ("Highlight Options", EditorStyles.boldLabel);
				EditorGUI.BeginChangeCheck ();
				if (!isManager) {
					if (!occluder.boolValue) {
						EditorGUILayout.PropertyField (ignore, new GUIContent ("Ignore", "This object won't be highlighted."));
						if (!ignore.boolValue) {
							EditorGUILayout.PropertyField (highlighted);
						}
					}
				}
				if (!ignore.boolValue) {
					EditorGUILayout.PropertyField (overlay);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField (overlayColor, new GUIContent ("Color"));
					EditorGUILayout.PropertyField (overlayBlending, new GUIContent ("Blending"));
					EditorGUILayout.PropertyField (overlayMinIntensity, new GUIContent ("Min Intensity"));
					EditorGUILayout.PropertyField (overlayAnimationSpeed, new GUIContent ("Animation Speed"));
					EditorGUILayout.PropertyField (overlayRenderQueue, new GUIContent ("Render Queue"));
					EditorGUI.indentLevel--;
					EditorGUILayout.PropertyField (outline);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField (outlineWidth, new GUIContent ("Width"));
					EditorGUILayout.PropertyField (outlineColor, new GUIContent ("Color"));
					EditorGUILayout.PropertyField (outlineSmooth, new GUIContent ("Smooth Edges"));
					EditorGUILayout.PropertyField (outlineQuality, new GUIContent ("Quality"));
					EditorGUILayout.PropertyField (outlineOnTop, new GUIContent ("Render On Top"));
                    EditorGUILayout.PropertyField (outlineExclusive, new GUIContent("Exclusive"));
                    EditorGUI.indentLevel--;
					EditorGUILayout.PropertyField (glow);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField (glowWidth, new GUIContent ("Width"));
					EditorGUILayout.PropertyField (glowAnimationSpeed, new GUIContent ("Animation Speed"));
					EditorGUILayout.PropertyField (glowSmooth, new GUIContent ("Smooth Edges"));
					EditorGUILayout.PropertyField (glowQuality, new GUIContent ("Quality"));
					EditorGUILayout.PropertyField (glowDithering, new GUIContent ("Dithering"));
					EditorGUILayout.PropertyField (glowOnTop, new GUIContent ("Render On Top"));
					if (glowDithering.boolValue) {
						EditorGUILayout.PropertyField (glowMagicNumber1, new GUIContent ("Magic Number 1"));
						EditorGUILayout.PropertyField (glowMagicNumber2, new GUIContent ("Magic Number 2"));
					}
					EditorGUILayout.PropertyField (glowPasses, true);
					EditorGUI.indentLevel--;
					EditorGUILayout.PropertyField (zoomScale);
				}

				EditorGUILayout.Separator ();
				EditorGUILayout.LabelField ("See-Through Options", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField (seeThrough);
				EditorGUI.indentLevel++;
				if (isManager && seeThrough.intValue == (int)HighlightEffect2D.SeeThroughMode.AlwaysWhenOccluded) {
					EditorGUILayout.HelpBox ("This option is not valid in Manager.\nTo make a sprite always visible add a Highlight Effect component to the sprite and enable this option on the component.", MessageType.Error);
				}
				EditorGUILayout.PropertyField (seeThroughIntensity, new GUIContent ("Intensity"));
				EditorGUILayout.PropertyField (seeThroughTintAlpha, new GUIContent ("Alpha"));
				EditorGUILayout.PropertyField (seeThroughTintColor, new GUIContent ("Color"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("Shadow Options", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField (shadow3D, new GUIContent ("3D Shadow"));
			if (!shadow3D.boolValue) {
				EditorGUILayout.PropertyField (shadowIntensity, new GUIContent ("Intensity"));
				EditorGUILayout.PropertyField (shadowColor, new GUIContent ("Color"));
				EditorGUILayout.PropertyField (shadowOffset, new GUIContent ("Offset"));
			}
			EditorGUI.indentLevel--
			;
			if (serializedObject.ApplyModifiedProperties ()) {
				foreach (HighlightEffect2D effect in targets) {
					effect.Refresh ();
				}
			}
			HighlightEffect2D _effect = (HighlightEffect2D)target;
			if (_effect != null && _effect.previewInEditor) {
				EditorUtility.SetDirty (_effect);
			}
		}


		[MenuItem ("GameObject/Effects/Highlight Plus 2D/Create Manager", false, 10)]
		static void CreateManager (MenuCommand menuCommand) {
			HighlightManager2D manager = FindObjectOfType<HighlightManager2D> ();
			if (manager == null) {
				GameObject managerGO = new GameObject ("HighlightPlus2DManager");
				manager = managerGO.AddComponent<HighlightManager2D> ();
				// Register root object for undo.
				Undo.RegisterCreatedObjectUndo (manager, "Create Highlight Plus 2D Manager");
			}
			Selection.activeObject = manager;
		}

	}

}