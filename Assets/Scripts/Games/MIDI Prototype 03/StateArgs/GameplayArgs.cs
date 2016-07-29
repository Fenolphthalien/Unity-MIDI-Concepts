using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeThree
{
	public class GameplayResultArgs : StateChangeEventArg
	{
		public readonly int playerHealth, totalFloorHealth, track;
		public readonly bool playerSuccessfull;
        public readonly UnityMIDI.MIDI midi;
		
		public GameplayResultArgs(GameStateControlBase sender, bool win, MIDI _midi, int _track) : base(sender)
		{
			playerSuccessfull = win;
			GameplayControl gamePlayControl = (GameplayControl)sender;
			if(gamePlayControl != null)
			{
				playerHealth = gamePlayControl.GetPlayerHealth();
				totalFloorHealth = gamePlayControl.GetFloorTotalHealth();
			}
            midi = _midi;
            track = _track;
		}
	}
}
