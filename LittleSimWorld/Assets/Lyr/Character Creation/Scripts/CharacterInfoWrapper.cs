using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace CharacterData.Wrappers {
	public class CharacterInfoWrapper {

		[OdinSerialize] string Name;
		[OdinSerialize] Gender gender;
		[OdinSerialize] Dictionary<CharacterPart, string> Visuals;



		public CharacterInfoWrapper(CharacterInfo info) {
			Visuals = Visuals.InitializeDefaultValues();
			var keys = info.SpriteSets.Keys;
			for (int i = 0; i < keys.Count; i++) {
				var element = info.SpriteSets.ElementAt(i);
				if (element.Value == null) { Visuals[element.Key] = ""; }
				else {
					Visuals[element.Key] = element.Value.name;
					//Debug.Log($"Saving {element.Value.name} as {element.Key}");
				}
			}

			Name = info.Name;
			gender = info.Gender;
		}

		public CharacterInfo GetVisuals() {
			CharacterInfo info = new CharacterInfo();
			info.Initialize();

			var keys = Visuals.Keys;
			foreach (var key in keys) {
				info.SpriteSets[key] = GetSetFromName(key, Visuals[key]);
			}

			info.Name = Name;
			info.Gender = gender;

			return info;
		}

		const string defaultSpriteSetPath = "Assets/Scriptable Objects/Resources/";

		CharacterSpriteSet GetSetFromName(CharacterPart key, string Name) {
			if (string.IsNullOrEmpty(Name)) { return null; }
			var path = $"Sprite Sets/{key}/{Name}";
			var obj = Resources.Load<CharacterSpriteSet>(path);
			//Debug.Log($"Loading {obj} as {key}");

			return obj;
		}
	}
}