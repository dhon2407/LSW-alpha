

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomOutlineShader : MonoBehaviour {


	SpriteRenderer outlineRenderer;
	Material outlineMaterial;

	public bool AutoSolveWidth = true;

	[HideIf("AutoSolveWidth")] public int MaxWidthInPixels;
	[HideIf("AutoSolveWidth")] public float Speed;


	void Awake() {
		MaxWidthInPixels = 40;
		Speed = 80;
		Initialize();
	}



	public bool isMouseOver;
	void Update() {
		if (!outlineRenderer) { return; }
		var currentWidth = outlineMaterial.GetFloat("_Width");

		float targetWidth;
		if (!isMouseOver) { targetWidth = Mathf.MoveTowards(currentWidth, 0, 2 * Speed * GameTime.Time.deltaTime); }
		else { targetWidth = Mathf.MoveTowards(currentWidth, MaxWidthInPixels, Speed * GameTime.Time.deltaTime); }

		outlineMaterial.SetFloat("_Width", targetWidth);
	}

	void Initialize() {

		var currentSprite = GetComponent<SpriteRenderer>().sprite;

		var texture = Resources.Load<Texture2D>("Empty_Texture");
		var size = new Rect(0, 0, texture.width, texture.height);

		var dif = new Vector2();
		dif.x = size.width - currentSprite.textureRect.width;
		dif.y = size.height - currentSprite.textureRect.height;

		if (dif.x <= 0 || dif.y <= 0) {
			Debug.Log("Sprite too big.. ");
			return;
		}

		var newPivot = new Vector2();
		newPivot.x = currentSprite.pivot.x / size.width;
		newPivot.y = currentSprite.pivot.y / size.height;

		if (currentSprite.pivot.y < 5) { newPivot.y = (dif.y / 2f) / size.height; }

		var spr = Sprite.Create(texture, size, newPivot, currentSprite.pixelsPerUnit);


		var sprGO = new GameObject();
		sprGO.name = "OutlineGO";
		sprGO.transform.parent = transform;
		sprGO.transform.localPosition = Vector3.zero + Vector3.up * 0.0001f;
		sprGO.transform.localScale = Vector3.one;

		outlineRenderer = sprGO.AddComponent<SpriteRenderer>();
		outlineRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
		outlineRenderer.spriteSortPoint = GetComponent<SpriteRenderer>().spriteSortPoint;
		outlineRenderer.sprite = spr;
		outlineRenderer.material = new Material(Shader.Find("Shader Graphs/OutlineRemappedTest"));

		outlineMaterial = outlineRenderer.material;
		outlineMaterial.SetFloat("_Width", 0);
		outlineMaterial.SetFloat("_MaxWidth", MaxWidthInPixels);
		outlineMaterial.SetTexture("_OriginalTex", currentSprite.texture);
		outlineMaterial.renderQueue = 2450;
	}

}



//	[SerializeField] bool ShowCachedSettings;
//	[ShowIf("ShowCachedSettings"), SerializeField] public Sprite spr;
//	[ShowIf("ShowCachedSettings"), SerializeField] SpriteRenderer outlineRenderer;
//	[ShowIf("ShowCachedSettings"), SerializeField] Material outlineMaterial;
//	[ShowIf("ShowCachedSettings"), SerializeField] string cachedString;

//	[ShowIf("ShowCachedSettings"), ShowInInspector] float _sprHeight => spr.textureRect.height;
//	[ShowIf("ShowCachedSettings"), ShowInInspector] float _sprWidth => spr.textureRect.width;

//	public bool AutoSolveWidth = true;

//	[HideIf("AutoSolveWidth")]
//	public int MaxWidthInPixels;

//	[HideIf("AutoSolveWidth")]
//	public float Speed;

//	void Awake() {
//		if (!spr) {
//			string errorString = $"{this.gameObject.name} is not Initialized properly..";
//			errorString += "Please Initialize using the Outline Initializer (search in Project).";
//			errorString += "Automatically initializing.. This takes time.";
//			Debug.LogError(errorString);
//			Destroy(this);
//			return;
//			//DestroyOldComponents();
//			//InitializeRenderer();
//		}

//		if (AutoSolveWidth) {
//			//Width = Mathf.RoundToInt(40f / transform.localScale.x);
//			AutoSolveWidth = false;
//		}

//		outlineMaterial = outlineRenderer.material;
//		outlineMaterial.SetTexture("_MainTex", spr.texture);
//		outlineMaterial.SetVector("_Dimensions", new Vector2(_sprWidth, _sprHeight));
//		outlineMaterial.SetFloat("_MaxWidth", MaxWidthInPixels);
//	}

//	public bool isMouseOver;
//	void Update() {
//		if (!spr) { return; }
//		var currentWidth = outlineMaterial.GetFloat("_Width");

//		float targetWidth;
//		if (!isMouseOver) { targetWidth = Mathf.MoveTowards(currentWidth, 0, 2 * Speed * Time.deltaTime); }
//		else { targetWidth = Mathf.MoveTowards(currentWidth, MaxWidthInPixels, Speed * Time.deltaTime); }

//		outlineMaterial.SetFloat("_Width", targetWidth);
//	}



//	#region Initialization
//#if UNITY_EDITOR
//	void DestroyOldComponents() {

//		//if (spr) { DestroyImmediate(spr); }
//		if (outlineRenderer) { DestroyImmediate(outlineRenderer.gameObject); }
//		//
//		spr = null;
//		outlineMaterial = null;

//	}

//	void CacheAssets() {
//		string path = $"Assets/Lyr/TempJunk/{gameObject.transform.parent.name}_{gameObject.name}_OutlineGO";
//		if (UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path + ".png") != null) {
//			string name = path + " ";
//			int s = 1;
//			while (true) {
//				if (UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(name + s + ".png") != null) { s++; }
//				else {
//					path = name + s + ".png";
//					break;
//				}
//			}
//		}
//		if (!path.Contains(".png")) { path += ".png"; }
//		if (!System.IO.Directory.Exists("Assets/Lyr/TempJunk/")) {
//			Debug.Log("Wrong Directory");
//			return;
//		}
//		if (System.IO.File.Exists(path)) {
//			Debug.Log("Something Wrong");
//			return;
//		}
//		Debug.Log("Creating File on " + path);

//		var bytes = outlineRenderer.sprite.texture.EncodeToPNG();
//		System.IO.File.WriteAllBytes(path, bytes);

//		cachedString = path;
//		AssetDatabase.ImportAsset(path);
//		AssetDatabase.Refresh();

//		TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
//		if (importer != null) {
//			var settings = new TextureImporterSettings();
//			importer.ReadTextureSettings(settings);
//			//{
//			//	var ___spr = GetComponent<SpriteRenderer>().sprite;
//			//	var ___path = AssetDatabase.GetAssetPath(___spr);
//			//	var tempImporter = AssetImporter.GetAtPath(path) as TextureImporter;
//			//	if (!tempImporter) {
//			//		Debug.Log("No importer");
//			//		return;
//			//	}
//			//
//			//	tempImporter.ReadTextureSettings(settings);
//			//}
//			settings.spriteMeshType = SpriteMeshType.FullRect;
//			settings.spritePixelsPerUnit = GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
//			settings.spritePivot = spr.pivot;

//			importer.SetTextureSettings(settings);
//			AssetDatabase.ImportAsset(path);
//			AssetDatabase.Refresh();
//		}
//		else {
//			Debug.Log("Null importer");
//			return;
//		}

//		var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
//		if (!sprite) { Debug.Log("No sprite"); }
//		else {
//			outlineRenderer.sprite = sprite;
//			spr = sprite;
//			return;
//		}
//		var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//		if (!tex) { Debug.Log("No texture"); }

//	}

//	void InitializeRenderer(GameObject outlinePrefab) {
//		spr = GetComponent<SpriteRenderer>().sprite;

//		if (!spr.texture.isReadable) {
//			Debug.Log($"Texture of {this.gameObject.name} isn't readable.");
//			Debug.Log($"You need to enable Read Write on {spr.texture.name} for the outline to work");
//			//this.enabled = false;
//			return;
//		}

//		if (!outlineRenderer) {
//			if (outlinePrefab == null) {
//				Debug.Log("Not initializing");
//				return;
//			}

//			var sprGO = GameObject.Instantiate(outlinePrefab);
//			sprGO.name = "OutlineGO";
//			sprGO.transform.parent = transform;
//			sprGO.transform.localPosition = Vector3.zero + Vector3.up * 0.0001f;
//			sprGO.transform.localScale = Vector3.one;

//			outlineRenderer = sprGO.GetComponent<SpriteRenderer>();
//			outlineRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
//			outlineRenderer.spriteSortPoint = GetComponent<SpriteRenderer>().spriteSortPoint;
//		}

//		int offset = 2 * MaxWidthInPixels;

//		var sprTexture = spr.texture;
//		var sprRect = spr.textureRect;

//		int sprWidth = (int) sprRect.size.x;
//		int sprHeight = (int) sprRect.size.y;

//		if (AutoSolveWidth) {

//			var scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
//			float _autoWidth = Mathf.Max(sprWidth, sprHeight) / (scale * 10f);
//			MaxWidthInPixels = (int) (_autoWidth);

//			Speed = 2 * MaxWidthInPixels;

//			offset = (int) (2 * _autoWidth);
//		}

//		else {


//		}

//		offset = 40;

//		var textureHeight = sprHeight + offset;
//		var textureWidth = sprWidth + offset;

//		var clearColor = Color.clear;

//		var texture = InitializeSprite();
//		ImportSprite(spr, 0, offset / 2);

//		var rect = new Rect(0, 0, textureWidth, textureHeight);

//		var newPivot = new Vector2();
//		newPivot.x = spr.pivot.x / sprRect.size.x;
//		newPivot.y = spr.pivot.y / sprRect.size.y;
//		if (spr.pivot.y < 5) { newPivot.y = (offset / 2f) / textureHeight; }


//		#region Attempt on reusing texture
//		// var newRect = spr.textureRect;
//		// newRect.size += new Vector2(offset, offset);
//		// newRect.position -= new Vector2(offset, offset) / 2;
//		// 
//		// 
//		// 
//		// outlineRenderer.sprite = Sprite.Create(spr.texture, newRect, newPivot, spr.pixelsPerUnit, 1, SpriteMeshType.FullRect);
//		// 
//		// Debug.Log("Initialized " + this.gameObject);
//		// 
//		// spr = outlineRenderer.sprite;
//		// 
//		// return;
//		#endregion

//		#region UNFINISHED SIZE OPTIMIZATION
//		/*
//		float rescale = 1 / 4f;
//		float newWidth = rescale * textureWidth;
//		float newHeight = rescale * textureHeight;
//		float newPPU = rescale * spr.pixelsPerUnit;

//		texture.Resize((int) newWidth, (int) newHeight, TextureFormat.Alpha8, false);
//		texture.Compress(true);
//		texture.Apply();

//		var newRect = new Rect(0, 0, texture.width, texture.height);

//		if (AutoSolveWidth) {

//			//var scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
//			float _autoWidth = Mathf.Max(texture.width, texture.height) / (2);
//			MaxWidthInPixels = (int) (_autoWidth);

//			Speed = 2 * MaxWidthInPixels;
//		}
//		*/
//		#endregion

//		//spr = Sprite.Create(texture, newRect, newPivot, newPPU, 1, SpriteMeshType.FullRect);


//		var tempTxtr = GetSceneAsset();
//		if (tempTxtr == null) {
//			Debug.Log("Failed to load asset");
//			return;
//		}

//		var tempSpr = Sprite.Create(tempTxtr, rect, newPivot, spr.pixelsPerUnit, 1, SpriteMeshType.FullRect);

//		outlineRenderer.sprite = tempSpr;
//		spr = tempSpr;

//		Debug.Log($"(W={MaxWidthInPixels}) - Initialized: {gameObject.name} with parent: {transform.parent.name}.");
//		Debug.Log($"Sprite Size: {spr.textureRect.width}x{spr.textureRect.height}.");


//		Texture2D InitializeSprite() {

//			Texture2D texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.Alpha8, false);

//			// Initialize Texture
//			for (int x = 0; x < textureWidth; x++) {
//				for (int y = 0; y < textureHeight; y++) {
//					texture2D.SetPixel(x, y, clearColor);
//				}
//			}

//			return texture2D;
//		}


//		void ImportSprite(Sprite spr, int offsetX, int offsetY) {

//			int ReadStartX = (int) sprRect.position.x;
//			int ReadEndX = ReadStartX + sprWidth;

//			int ReadStartY = (int) sprRect.position.y;
//			int ReadEndY = ReadStartY + sprHeight;

//			int WriteEndY = textureHeight - offsetY;
//			int WriteStartY = WriteEndY - sprHeight;

//			int WriteStartX = ((textureWidth - sprWidth) - offsetX) / 2;
//			int WriteEndX = textureWidth - WriteStartX - offsetX;

//			int ReadPixelsX = ReadStartX;
//			int ReadPixelsY = ReadStartY;

//			for (int x = WriteStartX; x <= WriteEndX; x++) {

//				ReadPixelsY = ReadStartY;

//				for (int y = WriteStartY; y <= WriteEndY; y++) {

//					// safety otherwise unity crashes
//					if (ReadPixelsX <= sprTexture.width && ReadPixelsY <= sprTexture.height) {
//						var color = sprTexture.GetPixel(ReadPixelsX, ReadPixelsY);
//						//if (color.a >= 0.8f) { texture.SetPixel(x, y, Color.white); }
//					}

//					ReadPixelsY++;
//					if (ReadPixelsY > ReadEndY) { break; }
//				}

//				ReadPixelsX++;
//				if (ReadPixelsX > ReadEndX) { break; }
//			}

//			//texture.Apply();
//			//texture.Compress(true);
//			//texture.Apply();
//		}

//		Texture2D GetSceneAsset() {
//			string path = $"Assets/Lyr/TempJunk/{gameObject.name}_{gameObject.transform.parent.name}_OutlineGO";

//			if (UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path + ".png") != null) {
//				string name = path + " ";
//				int s = 1;
//				while (true) {
//					if (UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(name + s + ".png") != null) { s++; }
//					else {
//						path = name + s + ".png";
//						break;
//					}
//				}
//			}
//			if (!path.Contains(".png")) { path += ".png"; }
//			if (!System.IO.Directory.Exists("Assets/Lyr/TempJunk/")) {
//				Debug.Log("Wrong Directory");
//				return null;
//			}
//			if (System.IO.File.Exists(path)) {
//				Debug.Log("Something Wrong");
//				return null;
//			}
//			Debug.Log("Creating File on " + path);

//			//var bytes = texture.EncodeToPNG();
//			//System.IO.File.WriteAllBytes(path, bytes);

//			cachedString = path;
//			AssetDatabase.ImportAsset(path);
//			AssetDatabase.Refresh();

//			return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
//		}


//	}
//#endif
//	#endregion

//	#region old code
//	//public SpriteRenderer RendererOfOutlinedObject;
//	//public GameObject  OutlineGO;
//	//[Range(0, 30)]
//	//public int outlineThickness = 10;
//	//public Color OutlineColor = new Color(255, 244, 0);
//	//
//	//bool _isMouseOver;
//	//public bool isMouseOver {
//	//	get => _isMouseOver;
//	//	set {
//	//		_isMouseOver = value;
//	//		OutlineGO.SetActive(_isMouseOver);
//	//	}
//	//}
//	//
//	//private void Awake()
//	//{
//	//    if(!RendererOfOutlinedObject)
//	//        RendererOfOutlinedObject = GetComponent<SpriteRenderer>();
//	//}
//	//
//	//private void Start()
//	//{
//	//    if (!OutlineGO) { 
//	//        GameObject prefab = (GameObject)Resources.Load("OutlinePrefab", typeof(GameObject));
//	//        OutlineGO = Instantiate<GameObject>(prefab, RendererOfOutlinedObject.transform.position, RendererOfOutlinedObject.transform.rotation, RendererOfOutlinedObject.transform);
//	//        SpriteRenderer outlineSR = OutlineGO.GetComponent<SpriteRenderer>();
//	//        outlineSR.sortingOrder = RendererOfOutlinedObject.sortingOrder;
//	//        outlineSR.spriteSortPoint = RendererOfOutlinedObject.spriteSortPoint;
//	//        outlineSR.sprite = RendererOfOutlinedObject.sprite;
//	//        
//	//        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
//	//        outlineSR.GetPropertyBlock(mpb);
//	//        mpb.SetFloat("_Outline", 1);
//	//        mpb.SetColor("_OutlineColor", OutlineColor);
//	//        mpb.SetFloat("_OutlineSize", outlineThickness);
//	//        outlineSR.SetPropertyBlock(mpb);
//	//    }
//	//
//	//    if (!OutlineGO.GetComponent<Animation>().isPlaying)
//	//        OutlineGO.GetComponent<Animation>().Play();
//	//
//	//	isMouseOver = false;
//	//}
//	#endregion


//}

