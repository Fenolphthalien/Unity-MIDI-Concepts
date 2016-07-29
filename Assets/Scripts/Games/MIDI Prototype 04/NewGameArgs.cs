using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeFour
{
	public class NewGameArgs : StateChangeEventArg
	{
		public readonly MIDI midi;
		public readonly int track;

		public NewGameArgs(BaseGameStateControl sender, MIDI _midi, int _track) : base(sender)
		{
			track = _track;
			midi = _midi;
		}
	}
}
