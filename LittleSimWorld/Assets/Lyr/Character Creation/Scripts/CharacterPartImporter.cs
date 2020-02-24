using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CharacterPartImporter : SerializedScriptableObject {

	#region Offset Settings

	int _offsetFromTopInPixels;
	int _offsetFromLeftInPixels;

	[ShowInInspector]
	public int OffsetFromTopInPixels {
		get => _offsetFromTopInPixels;
		set {
			_offsetFromTopInPixels = value;
			if (SpriteToPreview == null) { return; }
			var sprHeight = (int) SpriteToPreview.textureRect.height;
			_offsetFromTopInPixels = Mathf.Clamp(_offsetFromTopInPixels, 0, textureHeight - sprHeight);
		}
	}
	[ShowInInspector]
	public int OffsetFromBotInPixels {
		get {
			if (SpriteToPreview == null) { return 0; }
			var sprHeight = (int) SpriteToPreview.textureRect.height;

			return textureHeight - OffsetFromTopInPixels - sprHeight;
		}
		set {
			if (SpriteToPreview == null) { return; }
			var sprHeight = (int) SpriteToPreview.textureRect.height;
			OffsetFromTopInPixels = textureHeight - sprHeight - value;
		}
	}
	[ShowInInspector]
	public int HorizontalOffsetInPixels {
		get => _offsetFromLeftInPixels;
		set {
			_offsetFromLeftInPixels = value;
			if (SpriteToPreview == null) { return; }
			var sprWidth = (int) SpriteToPreview.textureRect.width;
			_offsetFromLeftInPixels = Mathf.Clamp(_offsetFromLeftInPixels, -(textureWidth - sprWidth), (textureWidth - sprWidth));
		}
	}

	#endregion

	public Sprite SpriteToPreview;

	[Space]
	public int textureWidth = 600;
	public int textureHeight = 1200;

	[Space]
	public BodyReferences defaults = new BodyReferences();


	[PreviewField(Alignment = ObjectFieldAlignment.Center, Height = 300f)]
	public Sprite sprite;

	[ShowInInspector] public int PixelWidth => SpriteToPreview?.texture?.width ?? 0;
	[ShowInInspector] public int PixelHeight => SpriteToPreview?.texture?.height ?? 0;
	[ShowInInspector] public Rect TextureRect => SpriteToPreview?.textureRect ?? new Rect();
	[ShowInInspector] public Vector2 TextureRectOffset => SpriteToPreview?.textureRectOffset ?? Vector2.zero;
	[ShowInInspector] public Vector4 TextureBorder => SpriteToPreview?.border ?? Vector4.zero;

	static Color clearColor = Color.clear;



	[Button]
	void Import() {
		var texture = InitializeSprite();
		ImportSprite(defaults.Body, texture, 0, textureHeight - (int)defaults.Body.textureRect.height);
		//ImportSprite(defaults.Head, texture, 0, 0);
		ImportSprite(SpriteToPreview, texture, _offsetFromLeftInPixels, _offsetFromTopInPixels);

		var rect = new Rect(0, 0, textureWidth, textureHeight);
		sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0));
	}


	Texture2D InitializeSprite() {
		Texture2D texture = new Texture2D(textureWidth, textureHeight);

		// Initialize Texture
		for (int x = 0; x < textureWidth; x++) {
			for (int y = 0; y < textureHeight; y++) {
				texture.SetPixel(x, y, clearColor);
			}
		}

		return texture;
	}

	void ImportSprite(Sprite spr, Texture2D texture, int offsetX, int offsetY) {

		var sprTexture = spr.texture;
		var sprRect = spr.textureRect;

		int sprWidth = (int) sprRect.size.x;
		int sprHeight = (int) sprRect.size.y;

		int ReadStartX = (int) sprRect.position.x;
		int ReadEndX = ReadStartX + sprWidth;

		int ReadStartY = (int) sprRect.position.y;
		int ReadEndY = ReadStartY + sprHeight;

		int WriteEndY = textureHeight - offsetY;
		int WriteStartY = WriteEndY - sprHeight;

		int WriteStartX = ((textureWidth - sprWidth) - offsetX) / 2;
		int WriteEndX = textureWidth - WriteStartX - offsetX;




		int ReadPixelsX = ReadStartX, ReadPixelsY = ReadStartY;

		for (int x = WriteStartX; x <= WriteEndX; x++) {

			ReadPixelsY = ReadStartY;

			for (int y = WriteStartY; y <= WriteEndY; y++) {

				// safety otherwise unity crashes
				if (ReadPixelsX <= sprTexture.width && ReadPixelsY <= sprTexture.height) {
					var color = sprTexture.GetPixel(ReadPixelsX, ReadPixelsY);
					if (color.a >= 0.8f) { texture.SetPixel(x, y, color); }
				}

				ReadPixelsY++;
				if (ReadPixelsY > ReadEndY) { break; }
			}

			ReadPixelsX++;
			if (ReadPixelsX > ReadEndX) { break; }
		}

		texture.Apply();
	}

	[HideReferenceObjectPicker]
	public class BodyReferences {
		[PreviewField] public Sprite Body;
		[PreviewField] public Sprite Head;

	}

}
