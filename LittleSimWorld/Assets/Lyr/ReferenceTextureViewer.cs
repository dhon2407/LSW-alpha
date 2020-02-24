using UnityEngine;

namespace Utilities.Testing {
	public class ReferenceTextureViewer : MonoBehaviour {
		[DrawTexture(500, 500)]
		public Texture2D ReferenceTexture;

		[DrawTexture(100)]
		public int x = 2;
	}
}
