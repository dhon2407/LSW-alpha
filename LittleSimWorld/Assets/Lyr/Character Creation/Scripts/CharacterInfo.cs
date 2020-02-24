using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;

namespace CharacterData {

	public enum Gender { Male, Female }

	public class CharacterInfo {

		public string Name;
		public Gender Gender;

		[HideReferenceObjectPicker]
		[ValidateInput("ValidateAllSlots", "Some slots are empty", InfoMessageType.Error)]
		[DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.CollapsedFoldout, IsReadOnly = true)]
		public Dictionary<CharacterPart, CharacterSpriteSet> SpriteSets = new Dictionary<CharacterPart, CharacterSpriteSet>();



		#region Editor Validation & Initialization

		bool ValidateAllSlots(Dictionary<CharacterPart, CharacterSpriteSet> dict, ref string errorMessage) {
			errorMessage = "";

			for (int i = 0; i < dict.Count; i++) {
				var element = dict.ElementAt(i);
				if (element.Value == null) { errorMessage += $"{element.Key} is empty.\n"; }
				else {
					bool x1 = element.Value.Bot == null;
					bool x2 = element.Value.Top == null;
					bool x3 = element.Value.Left == null;
					bool x4 = element.Value.Right == null;
					if (x1 || x2 || x3 || x4) { errorMessage += $"{element.Value} has empty slots"; }
				}
			}

			return errorMessage == "";
		}

		[Button, InfoBox("Warning, this will remove any existing references", InfoMessageType.Warning)]
		public void Initialize() {
			SpriteSets.InitializeDefaultValues(false);
			SpriteSets.Remove(CharacterPart.None);
		}
		#endregion

	}
}