using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.UI;

using UnityEngine;

namespace UI.CharacterCreation {
	using CharacterData;

	public enum HairColor { One, Two, Three, Four, Five }
	public enum UIAttributeType { None, Hair, Colour, Face, Facial, Top, Skin, Bottom, Other }
	public enum ArrowType { Left = -1, None = 0, Right = 1 }

	[DefaultExecutionOrder(-1)]
	public class CharacterCreationManager : SerializedMonoBehaviour {

		[HideReferenceObjectPicker] public static CharacterInfo CurrentCharacterInfo = null;
		[HideReferenceObjectPicker] public SpriteSetCollection Collection = new SpriteSetCollection();

		public static CharacterCreationManager instance;
		public event UIAction OnGenderChanged;


		[HideInInspector] public Gender gender = Gender.Male;
		int HairStyleIndex = 0, HairColorIndex = 0, ShirtIndex = 0, PantsIndex = 0;
		int BodyToneIndex = 0, FaceIndex = 0;

		void Awake() {
			instance = this;
			CurrentCharacterInfo = new CharacterInfo();
			CurrentCharacterInfo.Gender = Collection.gender = gender = Gender.Male;
		}

		public void ChangeGender(ArrowType arrow) {
			int sign = (int) arrow;
			if (sign != 0) {
				if (gender == Gender.Female) { gender = Gender.Male; }
				else { gender = Gender.Female; }
			}

			Collection.gender = gender;

			OnGenderChanged();
		}

		public void ChangeSprite(UIAttributeType type, ArrowType arrow, ref int index) {
			int sign = (int) arrow;
			//if (sign == 0) { Debug.Log("Arrow Type is 0.. Updating"); }

			switch (type) {
				case UIAttributeType.None:
					//Debug.Log("Wrong Type");
					break;

				case UIAttributeType.Hair:
					ClampInt(ref HairStyleIndex, Collection.Hairstyles.Count, sign, out index);

					PreviewManager.SetHair(Collection.Hairstyles[HairStyleIndex]);
					CurrentCharacterInfo.SpriteSets[CharacterPart.Hair] = Collection.Hairstyles[HairStyleIndex];
					break;

				case UIAttributeType.Colour:
					ClampInt(ref HairColorIndex, System.Enum.GetValues(typeof(HairColor)).Length, sign, out index);
					Collection.hairColor = (HairColor) HairColorIndex;

					PreviewManager.SetHair(Collection.Hairstyles[HairStyleIndex]);
					CurrentCharacterInfo.SpriteSets[CharacterPart.Hair] = Collection.Hairstyles[HairStyleIndex];
					break;

				case UIAttributeType.Face:
					ClampInt(ref FaceIndex, Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Face].Count, sign, out index);

					//PreviewManager.SetHair(Collection.Hairstyles[HairStyleIndex]);
					CurrentCharacterInfo.SpriteSets[CharacterPart.Face] = Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Face][0];
					break;

				case UIAttributeType.Facial:

					break;

				case UIAttributeType.Top:
					Collection.clothing = SpriteSetCollection.CharacterClothing.Top;
					ClampInt(ref ShirtIndex, Collection.Clothes.Count, sign, out index);

					PreviewManager.SetShirt(Collection.Clothes[ShirtIndex]);
					CurrentCharacterInfo.SpriteSets[CharacterPart.Top] = Collection.Clothes[ShirtIndex];
					break;

				case UIAttributeType.Skin:
					ClampInt(ref BodyToneIndex, 5, sign, out index);

					PreviewManager.SetBody(Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Body][BodyToneIndex]);
					PreviewManager.SetHead(Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Head][BodyToneIndex]);
					PreviewManager.SetHands(Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Hands][BodyToneIndex]);

					CurrentCharacterInfo.SpriteSets[CharacterPart.Body] = Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Body][BodyToneIndex];
					CurrentCharacterInfo.SpriteSets[CharacterPart.Head] = Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Head][BodyToneIndex];
					CurrentCharacterInfo.SpriteSets[CharacterPart.Hands] = Collection.SpriteSets[SpriteSetCollection.GeneralCharacterPart.Hands][BodyToneIndex];
					break;

				case UIAttributeType.Bottom:
					Collection.clothing = SpriteSetCollection.CharacterClothing.Bottom;
					ClampInt(ref PantsIndex, Collection.Clothes.Count, sign, out index);

					PreviewManager.SetPants(Collection.Clothes[PantsIndex]);
					CurrentCharacterInfo.SpriteSets[CharacterPart.Bottom] = Collection.Clothes[PantsIndex];
					break;

				case UIAttributeType.Other:
					break;
			}
		} 

		void ClampInt(ref int integer, int MaxValue, int adjustment, out int outIndex) {
			integer += adjustment;
			if (integer < 0) { integer = MaxValue - 1; }
			if (integer >= MaxValue) { integer = 0; }

			outIndex = integer;
		}

        public delegate void UIAction();
	}
}