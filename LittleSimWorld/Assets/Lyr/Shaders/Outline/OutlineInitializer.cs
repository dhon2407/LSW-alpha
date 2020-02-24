#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace InternalUtils {
	public class OutlineInitializer : SerializedScriptableObject {

		public GameObject OutlinePrefab;

		[InlineEditor, PreviewField, ShowInInspector, System.NonSerialized] public List<Sprite> sprites;



		[Button]
		public void InitializeAllOutlines() {
			var outlines = FindObjectsOfType<Outline>();
			sprites = new List<Sprite>();

			var _initializeRenderer = typeof(Outline).GetMethod("InitializeRenderer", BindingFlags.NonPublic | BindingFlags.Instance);
			var _destroyOldComponents = typeof(Outline).GetMethod("DestroyOldComponents", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var outlineObj in outlines) {

				_destroyOldComponents.Invoke(outlineObj, null);
				_initializeRenderer.Invoke(outlineObj, new[] { OutlinePrefab });

				EditorUtility.SetDirty(outlineObj.gameObject);
				EditorUtility.SetDirty(outlineObj);

				//sprites.Add(outlineObj.spr);
			}
		}

		[Button]
		void CacheStuff() {
			var outlines = FindObjectsOfType<Outline>();

			var _cacheAssets = typeof(Outline).GetMethod("CacheAssets", BindingFlags.NonPublic | BindingFlags.Instance);

			foreach (var outlineObj in outlines) {
				_cacheAssets.Invoke(outlineObj, null);

				EditorUtility.SetDirty(outlineObj.gameObject);
				EditorUtility.SetDirty(outlineObj);
			}
		}

		[Button]
		void Unload() {

			var f = FindObjectsOfType<Object>();
			foreach (var ff in f) {
				if (!(ff is Material || ff is Texture2D)) { continue; }
				if (!string.IsNullOrEmpty(ff.name)) { continue; }
				Debug.Log("Found Object Hanging" + ff);
				DestroyImmediate(ff);
			}
			return;

			//Resources.UnloadUnusedAssets();
			//var x = FindObjectsOfType<Sprite>();
			//foreach (var item in x) {
			//	Debug.Log("Destroying: " + item);
			//	//DestroyImmediate(item);
			//}
			//var y = FindObjectsOfType<Material>();
			//foreach (var item in x) {
			//	Debug.Log("Destroying Material: " + item);
			//	//DestroyImmediate(item);
			//}
			//var z = FindObjectsOfType<Texture2D>();
			//foreach (var item in z) {
			//	Debug.Log("Destroying texture: " + item);
			//}
		}
	}
}
#endif