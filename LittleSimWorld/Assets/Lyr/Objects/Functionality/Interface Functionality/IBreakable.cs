using System;
using Boo.Lang;
using Objects.Functionality;
using PlayerStats;
using Sirenix.OdinInspector;
using UnityEngine;
namespace Objects {
	/// <summary>
	/// <para>Interface for objects that can break and get fixed.</para>
	/// <para>Each contains a <see cref="BreakableFunctionality"/> which controls its functionality.</para>
	/// </summary>
	public interface IBreakable {
		GameObject gameObject { get; }

		/// <summary>
		/// <para>Functionality of an <see cref="IBreakable"/> object.</para>
		/// <para>Override the <see cref="BreakableFunctionality"/> class for custom functionality.</para>
		/// </summary>
		BreakableFunctionality breakFunctionality { get; }
	}

	namespace Functionality {
		/// <summary>
		/// <para>Functionality of an <see cref="IBreakable"/> object.</para>
		/// <para>Override this class for custom functionality.</para>
		/// </summary>
		[HideReferenceObjectPicker]
		public class BreakableFunctionality {

			[Tooltip("Chance to break per second.")]
			[Range(0, 100)] public int ChanceToBreak;
			[Range(0, 100)] public float RepairSpeed;

			[Space]

			public bool isBroken;
			[Range(0, 100), ShowIf("isBroken")]
			public float RepairProgress;

			[Space, Header("Functionality when object is broken")]
			public ParticleSystem BrokenParticleSystem;

			public AudioSource AudioSource;
			public AudioClip BrokenAudioClip;

			float previousBreakCheckTime;

			/// <summary> 
			/// <para>Calculates whether the object broke while being used or not. </para> 
			/// <para>The function returns true if the object breaks.</para> 
			/// <para>The calculation can happen once per second at maximum.</para>
			/// </summary>
			public bool DidBreakDuringLastUse() {
				if (isBroken) { return true; }

				if (previousBreakCheckTime >= Time.time - 1) { return isBroken; }
				previousBreakCheckTime = Time.time;

				// Break if the random number is small enough.
				int rnd = UnityEngine.Random.Range(0, 100);
				bool didObjectBreak = rnd <= ChanceToBreak;

				return didObjectBreak;
			}

			/// <summary>
			/// Start the particle system and set up the variables for <see cref="AttemptFix"/>.
			/// </summary>
			public virtual void Break() {
				// Have the broken particle system play
				if (BrokenParticleSystem) { BrokenParticleSystem.Play(); }

				// Have the audio clip play
				if (AudioSource) {
					AudioSource.clip = BrokenAudioClip;
					AudioSource.loop = false;
					AudioSource.time = 0;
					AudioSource.Play();
				}

				// Set up the variables
				isBroken = true;
				RepairProgress = 0;
			}

			/// <summary>
			/// Attempt to fix the broken object.
			/// <para>Returns true if the fix is completed.</para>
			/// </summary>
			/// <returns>True if the object has been fixed, false if it still needs fixing.</returns> 
			public virtual bool AttemptFix() {
				if (!isBroken) {
					Debug.LogError("Object is not broken.");
					return true;
				}

				RepairProgress += RepairSpeed * GameTime.Time.deltaTime;

				if (RepairProgress >= 100) {
					isBroken = false;
					RepairProgress = 0;

					if (BrokenParticleSystem) { BrokenParticleSystem.Stop(); }
					if (AudioSource) { AudioSource.Stop(); }

					return true;
				}

				return false;
			}

		}
	}
}