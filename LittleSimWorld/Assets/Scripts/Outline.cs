

using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer))]
public class Outline : MonoBehaviour
{

	[SerializeField] bool ShowCachedSettings;
	[ShowIf("ShowCachedSettings"), SerializeField] Sprite spr;
	[ShowIf("ShowCachedSettings"), SerializeField] SpriteRenderer outlineRenderer;
	[ShowIf("ShowCachedSettings"), SerializeField] Material outlineMaterial;
	[ShowIf("ShowCachedSettings"), ShowInInspector] float _sprHeight => spr.textureRect.height;
	[ShowIf("ShowCachedSettings"), ShowInInspector] float _sprWidth => spr.textureRect.width;

	public bool AutoSolveWidth = true;

	[HideIf("AutoSolveWidth")]
	public int MaxWidthInPixels;

	[HideIf("AutoSolveWidth")]
	public float Speed;

	void Awake() {
		if (!spr) {
			string errorString = $"{this.gameObject.name} is not Initialized properly..";
			errorString += "Please Initialize using the Outline Initializer (search in Project).";
			errorString += "Automatically initializing.. This takes time.";
			Debug.LogError(errorString);
			Destroy(this);
			return;
			//DestroyOldComponents();
			//InitializeRenderer();
		}

		if (AutoSolveWidth) {
			//Width = Mathf.RoundToInt(40f / transform.localScale.x);
			AutoSolveWidth = false;
		}

		outlineMaterial = outlineRenderer.material;
		outlineMaterial.SetTexture("_MainTex", spr.texture);
		outlineMaterial.SetVector("_Dimensions", new Vector2(_sprWidth, _sprHeight));
		outlineMaterial.SetFloat("_MaxWidth", MaxWidthInPixels);
	}

	public bool isMouseOver;
	void Update() {
		if (!spr) { return; }
		var currentWidth = outlineMaterial.GetFloat("_Width");

		float targetWidth;
		if (!isMouseOver) { targetWidth = Mathf.MoveTowards(currentWidth, 0, 2 * Speed * Time.deltaTime); }
		else { targetWidth = Mathf.MoveTowards(currentWidth, MaxWidthInPixels, Speed * Time.deltaTime); }

		outlineMaterial.SetFloat("_Width", targetWidth);
	}



	#region Initialization
#if UNITY_EDITOR
	void DestroyOldComponents() {

		spr = null;
		outlineMaterial = null;

		if (outlineRenderer) { DestroyImmediate(outlineRenderer.gameObject); }

	}

	void InitializeRenderer(GameObject outlinePrefab) {
		spr = GetComponent<SpriteRenderer>().sprite;

		if (!spr.texture.isReadable) {
			Debug.Log($"Texture of {this.gameObject.name} isn't readable.");
			Debug.Log($"You need to enable Read Write on {spr.texture.name} for the outline to work");
			//this.enabled = false;
			return;
		}

		if (!outlineRenderer) {

			var sprGO = GameObject.Instantiate(outlinePrefab);
			sprGO.name = "OutlineGO";
			sprGO.transform.parent = transform;
			sprGO.transform.localPosition = Vector3.zero + Vector3.up * 0.0001f;
			sprGO.transform.localScale = Vector3.one;

			outlineRenderer = sprGO.GetComponent<SpriteRenderer>();
			outlineRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
			outlineRenderer.spriteSortPoint = GetComponent<SpriteRenderer>().spriteSortPoint;
		}

		int offset = 2 * MaxWidthInPixels;

		var sprTexture = spr.texture;
		var sprRect = spr.textureRect;

		int sprWidth = (int) sprRect.size.x;
		int sprHeight = (int) sprRect.size.y;

		if (AutoSolveWidth) {

			var scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
			float _autoWidth = Mathf.Max(sprWidth, sprHeight) / (scale * 10f);
			MaxWidthInPixels = (int) (_autoWidth);

			Speed = 2 * MaxWidthInPixels;

			offset = (int) (2 * _autoWidth);
		}

		else {


		}

		var textureHeight = sprHeight + offset;
		var textureWidth = sprWidth + offset;

		var clearColor = Color.clear;

		var texture = InitializeSprite();
		ImportSprite(spr, 0, offset / 2);

		var rect = new Rect(0, 0, textureWidth, textureHeight);

		var newPivot = new Vector2();
		newPivot.x = spr.pivot.x / sprRect.size.x;
		newPivot.y = spr.pivot.y / sprRect.size.y;

		if (spr.pivot.y < 5) { newPivot.y = (offset / 2f) / textureHeight; }

		#region UNFINISHED SIZE OPTIMIZATION
		/*
		float rescale = 1 / 4f;
		float newWidth = rescale * textureWidth;
		float newHeight = rescale * textureHeight;
		float newPPU = rescale * spr.pixelsPerUnit;

		texture.Resize((int) newWidth, (int) newHeight, TextureFormat.Alpha8, false);
		texture.Compress(true);
		texture.Apply();

		var newRect = new Rect(0, 0, texture.width, texture.height);

		if (AutoSolveWidth) {

			//var scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
			float _autoWidth = Mathf.Max(texture.width, texture.height) / (2);
			MaxWidthInPixels = (int) (_autoWidth);

			Speed = 2 * MaxWidthInPixels;
		}
		*/
		#endregion

		//spr = Sprite.Create(texture, newRect, newPivot, newPPU, 1, SpriteMeshType.FullRect);

		spr = Sprite.Create(texture, rect, newPivot, spr.pixelsPerUnit, 1, SpriteMeshType.FullRect);
		outlineRenderer.sprite = spr;

		Debug.Log($"(W={MaxWidthInPixels}) - Initialized: {gameObject.name} with parent: {transform.parent.name}.");
		Debug.Log($"Sprite Size: {spr.textureRect.width}x{spr.textureRect.height}.");


		Texture2D InitializeSprite() {

			Texture2D texture2D = new Texture2D(textureWidth, textureHeight, TextureFormat.Alpha8, false);

			// Initialize Texture
			for (int x = 0; x < textureWidth; x++) {
				for (int y = 0; y < textureHeight; y++) {
					texture2D.SetPixel(x, y, clearColor);
				}
			}

			return texture2D;
		}


		void ImportSprite(Sprite spr, int offsetX, int offsetY) {

			int ReadStartX = (int) sprRect.position.x;
			int ReadEndX = ReadStartX + sprWidth;

			int ReadStartY = (int) sprRect.position.y;
			int ReadEndY = ReadStartY + sprHeight;

			int WriteEndY = textureHeight - offsetY;
			int WriteStartY = WriteEndY - sprHeight;

			int WriteStartX = ((textureWidth - sprWidth) - offsetX) / 2;
			int WriteEndX = textureWidth - WriteStartX - offsetX;

			int ReadPixelsX = ReadStartX;
			int ReadPixelsY = ReadStartY;

			for (int x = WriteStartX; x <= WriteEndX; x++) {

				ReadPixelsY = ReadStartY;

				for (int y = WriteStartY; y <= WriteEndY; y++) {

					// safety otherwise unity crashes
					if (ReadPixelsX <= sprTexture.width && ReadPixelsY <= sprTexture.height) {
						var color = sprTexture.GetPixel(ReadPixelsX, ReadPixelsY);
						if (color.a >= 0.8f) { texture.SetPixel(x, y, Color.white); }
					}

					ReadPixelsY++;
					if (ReadPixelsY > ReadEndY) { break; }
				}

				ReadPixelsX++;
				if (ReadPixelsX > ReadEndX) { break; }
			}

			texture.Apply();
			texture.Compress(true);
			texture.Apply();
		}
	}
#endif
	#endregion

	#region old code
	//public SpriteRenderer RendererOfOutlinedObject;
	//public GameObject  OutlineGO;
	//[Range(0, 30)]
	//public int outlineThickness = 10;
	//public Color OutlineColor = new Color(255, 244, 0);
	//
	//bool _isMouseOver;
	//public bool isMouseOver {
	//	get => _isMouseOver;
	//	set {
	//		_isMouseOver = value;
	//		OutlineGO.SetActive(_isMouseOver);
	//	}
	//}
	//
	//private void Awake()
	//{
	//    if(!RendererOfOutlinedObject)
	//        RendererOfOutlinedObject = GetComponent<SpriteRenderer>();
	//}
	//
	//private void Start()
	//{
	//    if (!OutlineGO) { 
	//        GameObject prefab = (GameObject)Resources.Load("OutlinePrefab", typeof(GameObject));
	//        OutlineGO = Instantiate<GameObject>(prefab, RendererOfOutlinedObject.transform.position, RendererOfOutlinedObject.transform.rotation, RendererOfOutlinedObject.transform);
	//        SpriteRenderer outlineSR = OutlineGO.GetComponent<SpriteRenderer>();
	//        outlineSR.sortingOrder = RendererOfOutlinedObject.sortingOrder;
	//        outlineSR.spriteSortPoint = RendererOfOutlinedObject.spriteSortPoint;
	//        outlineSR.sprite = RendererOfOutlinedObject.sprite;
	//        
	//        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
	//        outlineSR.GetPropertyBlock(mpb);
	//        mpb.SetFloat("_Outline", 1);
	//        mpb.SetColor("_OutlineColor", OutlineColor);
	//        mpb.SetFloat("_OutlineSize", outlineThickness);
	//        outlineSR.SetPropertyBlock(mpb);
	//    }
	//
	//    if (!OutlineGO.GetComponent<Animation>().isPlaying)
	//        OutlineGO.GetComponent<Animation>().Play();
	//
	//	isMouseOver = false;
	//}
	#endregion


}

