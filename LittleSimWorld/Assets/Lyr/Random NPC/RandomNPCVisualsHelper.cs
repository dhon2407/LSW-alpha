namespace Characters.RandomNPC {

using System.Collections.Generic;
using CharacterData;
using Sirenix.OdinInspector;
	using UnityEditor;
	using UnityEngine;
	using System.Linq;

[HideReferenceObjectPicker]
public class VisualsHelper {

		const float minFacePosition = -0.058f;
		const float maxFacePosition = -0.018f;

		public Dictionary<CharacterPart, SpriteRenderer> bodyParts;

		public SpriteRenderer Hand_L;
		public SpriteRenderer Hand_R;

		Dictionary<int, CharacterSpriteSet> SpriteSets;
		Dictionary<int, SpriteRenderer> _bodyParts;
		CharacterPart[] bodyKeys;

		public void Initialize() {
			RegisterKeys();
			CacheOptimization();
			AssignRandomSets();
			AssignRandomFace();
			UpdateVisuals(CharacterOrientation.Bot);
            
		}

		void CacheOptimization() {
			SpriteSets = new Dictionary<int, CharacterSpriteSet>(bodyKeys.Length);
			foreach (var key in bodyKeys) { SpriteSets.Add((int) key, null); }

			_bodyParts = new Dictionary<int, SpriteRenderer>(bodyKeys.Length);
			foreach (var key in bodyKeys) { _bodyParts.Add((int) key, bodyParts[key]); }
		}

		void AssignRandomSets() {
			int tone = Random.Range(0, 5);
			int Gender = Random.Range(0, 2);
			foreach (var key in bodyKeys) { SpriteSets[(int) key] = CharacterClothingManager.instance.GetRandom(tone, (Gender) Gender, key); }
			SpriteSets[(int) CharacterPart.Hands] = CharacterClothingManager.instance.GetRandom(tone, (Gender) Gender, CharacterPart.Hands);
		}
		void AssignRandomFace() {
			float faceT = Random.Range(0f, 1f);
			var facePos = bodyParts[CharacterPart.Face].transform.localPosition;
			facePos.y = Mathf.Lerp(minFacePosition, maxFacePosition, faceT);
			bodyParts[CharacterPart.Face].transform.localPosition = facePos;
		}

		void RegisterKeys() {
			var bodyKeys = bodyParts.Keys;
			var activeKeyList = new List<CharacterPart>(10);
			foreach (var key in bodyKeys) {
				if (bodyParts[key] == null) { continue; }
				activeKeyList.Add(key);
			}

			this.bodyKeys = activeKeyList.ToArray();
		}

		public void FaceUP() => UpdateVisuals(CharacterOrientation.Top);
		public void FaceDOWN() => UpdateVisuals(CharacterOrientation.Bot);
		public void FaceRIGHT() => UpdateVisuals(CharacterOrientation.Right);
		public void FaceLEFT() => UpdateVisuals(CharacterOrientation.Left);


		public void ResetOrientation() { previousOrientation = -1; }
		int previousOrientation = -1;

		public void UpdateVisuals(CharacterOrientation orientation) {

			if (previousOrientation == (int) orientation) { return; }
			previousOrientation = (int) orientation;

			foreach (var key in bodyKeys) {
				var _key = (int) key;
				var part = _bodyParts[_key];
				var set = SpriteSets[_key];

				part.sprite = set.Get(orientation);
			}

			var handSpr = SpriteSets[(int) CharacterPart.Hands].Get(orientation);

			Hand_L.sprite = handSpr;
			Hand_R.sprite = handSpr;
		}

	}
}