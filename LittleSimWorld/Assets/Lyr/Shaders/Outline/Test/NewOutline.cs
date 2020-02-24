using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class NewOutline : MonoBehaviour
{
	public Material outlineMat;
	public SpriteRenderer outlineRenderer;

	public SpriteRenderer referenceSpriteRenderer;
	void Start() {
		outlineMat = outlineRenderer.material;
		var spr = GetComponent<SpriteRenderer>().sprite;
		var uv = spr.uv;

		outlineMat.SetTexture("_OriginalTex", spr.texture);
		outlineMat.SetVector("_uvBegin", uv[0]);
		outlineMat.SetVector("_uvEnd", uv[uv.Length - 1]);

		foreach (var _uv in uv) {
			Debug.Log("Content of uv: " + _uv);
		}
	}

	[Button]void PrintUVs() {
		var spr = referenceSpriteRenderer.sprite;
		var uv = spr.uv;

		outlineRenderer.sprite.pivot.Set(referenceSpriteRenderer.sprite.pivot.x, referenceSpriteRenderer.sprite.pivot.y);

		foreach (var _uv in uv) { Debug.Log("Content of uv: " + _uv.ToString("F6"));		}
	}

	[Button]
	void ApplyUV() {
		var spr = referenceSpriteRenderer.sprite;
		var txtR = spr.textureRect;

		var min = new Vector2(txtR.x, txtR.y);
		var max = min + new Vector2(txtR.width, txtR.height);

		min.x /= spr.texture.width;
		min.y /= spr.texture.height;

		max.x /= spr.texture.width;
		max.y /= spr.texture.height;

		outlineMat.SetTexture("_OriginalTex", spr.texture);
		outlineMat.SetVector("_uvBegin", min);
		outlineMat.SetVector("_uvEnd", max); 
	}
	Rect getUVs(Sprite sprite) {
		Rect UVs = sprite.rect;//It's important to note that Rect is a value type because it is a struct, so this copies the Rect.  You don't want to change the original.
		UVs.x /= sprite.texture.width;
		UVs.width /= sprite.texture.width;
		UVs.y /= sprite.texture.height;
		UVs.height /= sprite.texture.height;
		return UVs;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
