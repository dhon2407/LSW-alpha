using System.Collections;
using System.Collections.Generic;
using PathFinding;
using UnityEngine;

namespace Characters.RandomNPC {
	public enum CommandInterval { Update, FixedUpdate }

	public interface INPCCommand {
		CommandInterval interval { get; }
		bool IsFinished { get; set; }
		void ExecuteCommand();
		void Initialize();
	}
}