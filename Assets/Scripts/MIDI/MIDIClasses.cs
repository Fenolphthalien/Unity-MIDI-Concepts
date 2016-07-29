using UnityEngine;
using System.Collections;


namespace UnityMIDI{

	public abstract class MIDIBroadcaster : MonoBehaviour {}

	public abstract class MIDIMaskedReciever : MonoBehaviour
	{
		public Tone note = Tone.C;
		public int Octave = 3;
	}
}
