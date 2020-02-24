using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using global::Utilities;

namespace Objects.Functionality {
	[DrawChildElements, HideReferenceObjectPicker]
	public abstract class ActionEffect {
		public abstract void Activate();
		public abstract void Deactivate(bool canceled);
	}
	public class EnableObjectEffect : ActionEffect {
		[ShowInInspector, HideLabel, PropertyOrder(-1), ShowIf("type", ObjectEffectType.ByReference)]
		string ToolTip1 {
			get {
				string GetState(bool x) => !x ? "Enable" : "Disable";
				return $"{GetState(DisableOnBegin)} object on action begin, and {GetState(!DisableOnBegin)} it on end.";
			}
		}

		[ShowInInspector, HideLabel, PropertyOrder(-1), ShowIf("type", ObjectEffectType.ByName), MultiLineProperty(3)]
		string ToolTip2 {
			get {
					if (ObjectMethodPicker == null) { ObjectMethodPicker = new AbstractObjectMethodCaller(); }
					return $"Scan the scene for {ObjectMethodPicker.ObjectName},\nGet its component {ObjectMethodPicker.ObjectComponent?.Name},\nCall {ObjectMethodPicker.MethodToCall}().";
			}
		}

		public enum ObjectEffectType { ByReference, ByName}
		[EnumToggleButtons] public ObjectEffectType type;

		[Space, ShowIf("type", ObjectEffectType.ByReference)] public GameObject ObjectEffect;
		[BoxGroup("Abstract object method caller"), ShowIf("type", ObjectEffectType.ByName)]
		public AbstractObjectMethodCaller ObjectMethodPicker = new AbstractObjectMethodCaller();

		[Tooltip("Should the object be disabled instead in the begining?")]
		[FitLabelWidth,LabelText("Should disable object on begin:")] public bool DisableOnBegin;
		public override void Activate() {
			switch (type) {
				case ObjectEffectType.ByReference:
					ObjectEffect.SetActive(!DisableOnBegin);
					break;
				case ObjectEffectType.ByName:
					ObjectMethodPicker.Invoke();
					break;
			}
		}
		public override void Deactivate(bool canceled) => ObjectEffect.SetActive(DisableOnBegin);

	}
	public class PlaySoundEffect : ActionEffect {

		[ShowInInspector, HideLabel, PropertyOrder(-1)] string ToolTip => $"Play a sound clip.";

		public GameSettings.GameAudioSource source;

		[Space]
		public AudioClip OnBegin;
		public AudioClip DuringUse;
		public AudioClip OnEnd;
		public AudioClip OnCancel;

		public override void Activate() {
			if (!source) { Debug.LogWarning("No audio source found on PlaySoundEffect. Audio won't play."); return; }
			if (OnBegin) { source.PlaySound(OnBegin); }
			if (DuringUse) { source.PlaySound(DuringUse, true); }

		}
		public override void Deactivate(bool canceled) {
			if (!source) { Debug.LogWarning("No audio source found on PlaySoundEffect. Audio won't play."); return; }
			if (DuringUse) { source.StopPlayingSound(DuringUse); }

			if (canceled && OnCancel) { source.PlaySound(OnCancel); }
			else if (!canceled && OnEnd) { source.PlaySound(OnEnd); }

		}
	}
	public class PlayerAnimationEffect : ActionEffect {
		[ShowInInspector, HideLabel, PropertyOrder(-1)] string ToolTip => $"Change the animator's state.";

		public enum AnimationType { SetBool, PlayImmediately }

		[InfoBox(@"Animation effects with type: 'SetBool' will enable the bool in the begining and disable it in the end.")]
		[EnumToggleButtons] public AnimationType type;

		public string AnimationName;

		public override void Activate() {
			switch (type) {
				case AnimationType.SetBool:
					Player.anim.SetBool(AnimationName, true);
					break;
				case AnimationType.PlayImmediately:
					Player.anim.Play(AnimationName);
					break;
			}
		}
		public override void Deactivate(bool canceled) {
			switch (type) {
				case AnimationType.SetBool:
					Player.anim.SetBool(AnimationName, false);
					break;
				case AnimationType.PlayImmediately:
					//GameLibOfMethods.animator.Play(AnimationName);
					break;
			}
		}
	}

	public class PopUpUIEffect : ActionEffect {
		[ShowInInspector, HideLabel, PropertyOrder(-1)] string ToolTip => $"Pop up a floating UI text message.";

		public enum PopUpUIEffectType { OnBegin, OnEnd }
		[EnumToggleButtons] public PopUpUIEffectType type;

		[Tooltip("Plays if action processes normally.")]
		public string Text;

		[Tooltip("Plays if action terminates absurdly.")]
		[ShowIf("type", PopUpUIEffectType.OnEnd)]
		public string CanceledText;

		public override void Activate() {
			if (type == PopUpUIEffectType.OnEnd) { return; }
			GameLibOfMethods.CreateFloatingText(Text, 2);
		}
		public override void Deactivate(bool canceled) {
			if (type == PopUpUIEffectType.OnBegin) { return; }
			if (canceled) {
				if (string.IsNullOrWhiteSpace(CanceledText)) { return; }
				GameLibOfMethods.CreateFloatingText(CanceledText, 2);
			}
			else { GameLibOfMethods.CreateFloatingText(Text, 2); }
		}
	}

	public class ChangeGameTimeEffect : ActionEffect {
		[ShowInInspector, HideLabel, PropertyOrder(-1)] string ToolTip => $"Change game time.";
		[Range(0, 50)] public float TimeOnBegin = 1;
		[Range(0, 50)] public float TimeOnEnd = 1;
		public override void Activate() => GameTime.Clock.ChangeSpeed(TimeOnBegin);
		public override void Deactivate(bool canceled) => GameTime.Clock.ChangeSpeed(TimeOnEnd);
	}
}