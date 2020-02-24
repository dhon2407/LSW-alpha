using System.Collections;
using System.Collections.Generic;
using CharacterData;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterCreation {

	[HideReferenceObjectPicker]
	public class SpriteSetCollection {

		public enum GeneralCharacterPart {
			Head = 1,
			Face = 2,
			Facial = 3,
			Body = 5,
			Hands = 7,
			Other = 8
		}

		public enum CharacterClothing {
			Top = 4,
			Bottom = 6
		}


		[HideInInspector] public Dictionary<GeneralCharacterPart, List<CharacterSpriteSet>> SpriteSets;
		[HideInInspector] public Dictionary<HairColor, List<CharacterSpriteSet>> MaleHair;
		[HideInInspector] public Dictionary<HairColor, List<CharacterSpriteSet>> FemaleHair;
		[HideInInspector] public Dictionary<CharacterClothing, List<CharacterSpriteSet>> MaleClothes;
		[HideInInspector] public Dictionary<CharacterClothing, List<CharacterSpriteSet>> FemaleClothes;

		[EnumToggleButtons, PropertyOrder(9)]
		public GeneralCharacterPart bodyPart = GeneralCharacterPart.Body;
		[EnumToggleButtons, PropertyOrder(11)]
		public Gender gender = Gender.Male;
		[Space, EnumToggleButtons, PropertyOrder(12)]
		public HairColor hairColor = HairColor.One;
		[EnumToggleButtons, PropertyOrder(21)]
		public CharacterClothing clothing = CharacterClothing.Top;

		string BodyPartsLabel => $"{bodyPart} parts";
		string HairStylesLabel => $"{gender}'s Hair Set {hairColor}";
		string ClothesLabel => $"{gender}'s {clothing} clothes";

		// Display the lists based on the selection of the enums,
		[ShowInInspector, HideReferenceObjectPicker, PropertyOrder(10), LabelText("@BodyPartsLabel")]
		public List<CharacterSpriteSet> BodyParts {
			get {
				if (SpriteSets == null) { InitializeSpriteSets(); }
				if (bodyPart == 0) { bodyPart = GeneralCharacterPart.Body; }
				return SpriteSets[bodyPart];
			}
			set { }
		}
		[ShowInInspector, HideReferenceObjectPicker, PropertyOrder(20), LabelText("@HairStylesLabel")]
		public List<CharacterSpriteSet> Hairstyles {
			get {
				if (MaleHair == null || FemaleHair == null) { InitializeHair(); }

				if (gender == Gender.Male) { return MaleHair[hairColor]; }
				else { return FemaleHair[hairColor]; }
			}
			set { }
		}
		[ShowInInspector, HideReferenceObjectPicker, PropertyOrder(30), LabelText("@ClothesLabel")]
		public List<CharacterSpriteSet> Clothes {
			get {
				if (MaleClothes == null || FemaleClothes == null) { InitializeClothes(); }
				if (clothing == 0) { clothing = CharacterClothing.Bottom; }

				if (gender == Gender.Male) { return MaleClothes[clothing]; }
				else { return FemaleClothes[clothing]; }
			}
			set { }
		}


		#region Initialization
		//[PropertyOrder(1000), Button, InfoBox("Warning, this will remove any existing references", InfoMessageType.Warning)]
		//void Initialize() {
		//	InitializeSpriteSets();
		//	InitializeHair();
		//	InitializeClothes();
		//}

		void InitializeSpriteSets() {
			bodyPart = GeneralCharacterPart.Body;
			SpriteSets = new Dictionary<GeneralCharacterPart, List<CharacterSpriteSet>>();
			SpriteSets.InitializeDefaultValues(true);
		}
		void InitializeHair() {
			MaleHair = new Dictionary<HairColor, List<CharacterSpriteSet>>();
			MaleHair.InitializeDefaultValues(true);

			FemaleHair = new Dictionary<HairColor, List<CharacterSpriteSet>>();
			FemaleHair.InitializeDefaultValues(true);
		}
		void InitializeClothes() {
			clothing = CharacterClothing.Top;
			MaleClothes = new Dictionary<CharacterClothing, List<CharacterSpriteSet>>();
			MaleClothes.InitializeDefaultValues(true);

			FemaleClothes = new Dictionary<CharacterClothing, List<CharacterSpriteSet>>();
			FemaleClothes.InitializeDefaultValues(true);
		}
		#endregion
	}


}