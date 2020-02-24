using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using PlayerStats;

public class StatusUIUpdater : SerializedMonoBehaviour
{
	[Header("Player Info")]
	public TextMeshProUGUI NameText;
	public TextMeshProUGUI JobText;

	[HideReferenceObjectPicker] public PortraitUIUpdater portraitUI;
	[HideReferenceObjectPicker] public StatusBarUIUpdater statusBarUI;



	public static StatusUIUpdater instance;

	void Awake() {
		instance = this;
		statusBarUI.Initialize();
	}

	void Update() {
		statusBarUI.Update();
	}

	public static void UpdateEverything() { instance.UpdateNow(); }
	void UpdateNow() {
		NameText.text = SpriteControler.Instance.visuals.Name;
		JobText.text = JobManager.Instance.CurrentJob?.name ?? "Unemployed";

		portraitUI.Update();
		statusBarUI.Update();
	}


	public class PortraitUIUpdater {
		public Dictionary<CharacterData.CharacterPart, Image> CharacterPortraitImages;

		public PortraitUIUpdater() { CharacterPortraitImages = CharacterPortraitImages.InitializeDefaultValues(); }

		public void Update() {
			var characterSpriteSets = SpriteControler.Instance.visuals.SpriteSets;
			var keys = characterSpriteSets.Keys;

			foreach (var key in keys) {
				if (CharacterPortraitImages.ContainsKey(key) && CharacterPortraitImages[key])
					CharacterPortraitImages[key].sprite = characterSpriteSets[key].Bot;
			}
		}
	}

	public class StatusBarUIUpdater {

		[Header("Color Settings")]
		[HideReferenceObjectPicker] public Gradient gradient;
		[FitLabelWidth] public float MinimumPercentageToActivateGlowEffects;

		public Dictionary<Status.Type, StatusImageData> StatusBarSlots;
		Dictionary<Status.Type, float> _cachedPercentages;

		public StatusBarUIUpdater() {
			StatusBarSlots = StatusBarSlots.InitializeDefaultValues(true);
		}

		public void Initialize() {
			_cachedPercentages = _cachedPercentages.InitializeDefaultValues(-1f);
		}

		public void Update() {
			var keys = StatusBarSlots.Keys;

			foreach (var key in keys) {
				var maxAmount = Stats.Status(key).MaxAmount;
				var T = Stats.Status(key).CurrentAmount / maxAmount;

				// Only update if needed for performance.
				if (T == _cachedPercentages[key]) { continue; }

				var element = StatusBarSlots[key];

				element.image.fillAmount = T;
				element.image.color = gradient.Evaluate(1 - T);
				element.glowEffect.keepGoing = (T <= MinimumPercentageToActivateGlowEffects);

				// Update Cached value
				_cachedPercentages[key] = T;
			}
		}

		[HideReferenceObjectPicker]
		public class StatusImageData {
			public Image image;
			public GlowEffect glowEffect;
            public Image IconOfTheStatus;
            public Color ColorForBubbles;
		}

	}
}
