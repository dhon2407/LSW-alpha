using System.Collections.Generic;
using CharacterData;
using Sirenix.OdinInspector;
using UI.CharacterCreation;
using UnityEngine;

public class CharacterClothingManager : SerializedScriptableObject {

	#region Singleton SO

	static CharacterClothingManager _instance;
	public static CharacterClothingManager instance {
		get {
			if (!_instance) { _instance = Resources.Load<CharacterClothingManager>("Clothes Manager"); }
			return _instance;
		}
	}

	#endregion

	public SpriteSetCollection collection;

	public CharacterSpriteSet GetRandom(int tone, Gender gender, CharacterPart part) {
		collection.gender = gender;
		collection.clothing = (SpriteSetCollection.CharacterClothing) part;

		switch (part) {
			case CharacterPart.None:
				Debug.Log("uh.. wrong, guess again..");
				return null;
			case CharacterPart.Hair:
				collection.hairColor = (HairColor) Random.Range(0, 5);
				return RandomFromList(collection.Hairstyles);
			case CharacterPart.Head:
				return collection.SpriteSets[(SpriteSetCollection.GeneralCharacterPart) part][tone];
			case CharacterPart.Face:
				return collection.SpriteSets[(SpriteSetCollection.GeneralCharacterPart) part][0];
			case CharacterPart.Facial:
				//Debug.Log("No facials yet");
				return null;
			case CharacterPart.Top:
				return RandomFromList(collection.Clothes);
			case CharacterPart.Body:
				return collection.SpriteSets[(SpriteSetCollection.GeneralCharacterPart) part][tone];
			case CharacterPart.Bottom:
				return RandomFromList(collection.Clothes);
			case CharacterPart.Hands:
				return collection.SpriteSets[(SpriteSetCollection.GeneralCharacterPart) part][tone];
			case CharacterPart.Other:
				//Debug.Log("No other yet");
				return null;
			default:
				break;
		}

		return null;
	}

	CharacterSpriteSet RandomFromList(List<CharacterSpriteSet> list) => list[Random.Range(0, list.Count)];
}
