using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeThree
{
	public class NewGameArgs : StateChangeEventArg
	{
		public readonly MIDI midi;
		public readonly int track;

		public NewGameArgs(GameStateControlBase sender, MIDI _midi, int _track) : base(sender)
		{
			track = _track;
			midi = _midi;
		}
	}
}
