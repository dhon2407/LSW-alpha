#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace CharacterData {
	public class SpriteAssetCreator : Utilities.ScriptableObjectCreator<CharacterSpriteSet> {

		[ShowInInspector]
		CharacterPart SetType { get; set; }

		protected override string SavePath => "Assets/ScriptableObjects/Resources/Sprite Sets/" + SetType.ToString();

		protected override void AdditionalInitialization(CharacterSpriteSet obj) { }
		protected override bool AdditionalValidation() {
			if (SetType == CharacterPart.None) {
				Debug.Log("Select a Set Type from above");
				return false;
			}
			return true;
		}

		protected override void AdditionalReset() { }// => SetType = CharacterPart.None;


		
		[Header("Mass Import")]
		[PropertyOrder(999)] public string Prefix;
		[PropertyOrder(1000)] public int NextIndex;
		[PropertyOrder(1001)] public CharacterPart part;
		[PropertyOrder(1002)] public List<Sprite> SpriteCollection = new List<Sprite>();

		[Button]
		[PropertyOrder(1002)]
		void MassSpawn() {

			for (int i = 0; i < 5; i++) {
				item.Bot = SpriteCollection[i];
				item.Top = SpriteCollection[i];
				item.Right = SpriteCollection[i];
				item.Left = SpriteCollection[i];

				item.Part = part;
				SetType = part;

				Name = GetName(i);

				CreateAsset();
				NextIndex++;
			}


			SpriteCollection.Clear();
		}

		string GetName(int i) {
			string s = Prefix + " ";

			if (i == 0) { s += "Tone 1"; }
			if (i == 1) { s += "Tone 2"; }
			if (i == 2) { s += "Tone 3"; }
			if (i == 3) { s += "Tone 4"; }
			if (i == 4) { s += "Tone 5"; }

			return s;
		}
		
	}
}
#endif
