using System.Collections;
using System.Collections.Generic;
using CharacterData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {
	public class PreviewManager : MonoBehaviour {

		public static PreviewManager instance;

		public Image Body, Hair, Head, Shirt, Pants;
		public Image Hand_Left, Hand_Right;

		void Awake() { instance = this; }


		public static void SetHair(CharacterSpriteSet set) {
			instance.Hair.sprite = set.Bot;
		}
		public static void SetBody(CharacterSpriteSet set) {
			instance.Body.sprite = set.Bot;
		}
		public static void SetHead(CharacterSpriteSet set) {
			instance.Head.sprite = set.Bot;
		}
		public static void SetShirt(CharacterSpriteSet set) {
			instance.Shirt.sprite = set.Bot;
		}
		public static void SetPants(CharacterSpriteSet set) {
			instance.Pants.sprite = set.Bot;
			instance.Pants.gameObject.SetActive(set.Bot != null);
		}
		public static void SetHands(CharacterSpriteSet set) {
			instance.Hand_Left.sprite = set.Bot;
			instance.Hand_Right.sprite = set.Bot;
		}
	}
}