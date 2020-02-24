using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;

public enum CharacterOrientation { Bot, Top, Right, Left }

namespace CharacterData {
	[HideReferenceObjectPicker,InlineEditor]
	public class CharacterSpriteSet : SerializedScriptableObject {

		[HideInInlineEditors] public CharacterPart Part;

		const int size = 55;

		[PreviewField, HorizontalGroup("A", width: size), TitleGroup("A/Bot", alignment: TitleAlignments.Centered), HideLabel] public Sprite Bot;
		[PreviewField, HorizontalGroup("A", width: size), TitleGroup("A/Top", alignment: TitleAlignments.Centered), HideLabel] public Sprite Top;
		[PreviewField, HorizontalGroup("A", width: size), TitleGroup("A/Right", alignment: TitleAlignments.Centered), HideLabel] public Sprite Right;
		[PreviewField, HorizontalGroup("A", width: size), TitleGroup("A/Left", alignment: TitleAlignments.Centered), HideLabel] public Sprite Left;

		#region Accessibility 
		
		public Sprite Get(CharacterOrientation orientation) {
			switch (orientation) {
				case CharacterOrientation.Bot:
					return Bot;
				case CharacterOrientation.Top:
					return Top;
				case CharacterOrientation.Right:
					return Right;
				case CharacterOrientation.Left:
					return Left;
			}

			// Just so compiler compiles
			return null;
		}


		#endregion

	}
}