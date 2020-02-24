using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterData;
using Sirenix.OdinInspector;

namespace Characters {


	/// <summary>
	/// Script to derive Characters from
	/// </summary>
	public abstract class BaseCharacter : SerializedMonoBehaviour {

		public CharacterData.CharacterInfo characterInfo;
		//public CharacterData.CharacterStats characterStats;

	}

	public abstract class BaseNPC : BaseCharacter {

		// public NPCIdentity identity;
		// public JobInfo job;
		// public RelationshipTree relationships;
		// public List<DialogueLine> dialogue;
		// public TaskSchedule schedule;

		// TODO: Add when Pathfinding has been implemented
		public void MoveTo(Vector3 Position) {

		}

		IEnumerator<float> StartMovingTowards() {
			// TODO: Add when Pathfinding has been implemented
			yield return 0f;
		}

	}


}