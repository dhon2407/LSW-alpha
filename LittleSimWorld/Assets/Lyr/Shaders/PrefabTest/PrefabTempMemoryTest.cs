using UnityEngine;

public class PrefabTempMemoryTest : MonoBehaviour
{
	// The reference to the temporary sprite
	public Sprite spr;

	// The previous state of the sprite
	public bool shouldHaveSprite;

	[Space, Header("Toggle")]
	// Use this bool to create new sprite
	public bool ToggleMakeNewSprite;

	void OnValidate() {

		// Notify when the reference is lost
		if (shouldHaveSprite && !spr) {
			Debug.Log("Should have sprite but sprite reference is Lost");
			shouldHaveSprite = false;
			// ps: Sometimes the inspector won't update while you're in the "Prefab Edit" mode.
			// You will need to reopen the prefab for the changes to update
		}

		// Use the 'ToggleMakeNewSprite' bool as a toggle to create new sprite
		if (ToggleMakeNewSprite) {
			if (!spr) {
				SaveNewSprite();
				ToggleMakeNewSprite = false;
				shouldHaveSprite = true;
			}
			else {
				Debug.Log("Sprite reference is already stored");
				ToggleMakeNewSprite = false;
				shouldHaveSprite = true;
			}
		}
	}

	// Call this method on the prefab GameObject to see actual behaviour.
	// Call this on the Instantiated object that is residing inside the scene for expected behaviour.
	void SaveNewSprite() {
		spr = Sprite.Create(new Texture2D(100, 100), new Rect(), Vector2.one * 0.5f);
		Debug.Log("Temporary Sprite created");

		// Potentially useful methods to look at (they have no effect)
		//EditorUtility.SetDirty(this);
		//EditorUtility.SetDirty(this.gameObject);
	}
}
