using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace UnityMIDI
{
	//When a note on event occurs.
	public interface INoteOnHandler : IMIDIEventHandler
	{
		void OnNoteOn (MIDIMessage midiMessage);
	}

	//When a note off event occurs.
	public interface INoteOffHandler : IMIDIEventHandler
	{
		void OnNoteOff (MIDIMessage midiMessage);
	}

    //While a note is on - This dependends on the controller.
    public interface IAftertouchHandler : IMIDIEventHandler
	{
        void OnAftertouch(MIDIMessage midiMessage);
    }

	public interface IMIDIEventHandler : IEventSystemHandler{}

	//For the metronome class.
	public interface IBeatEventHandler : IEventSystemHandler{}

	public interface IBeatHandler : IBeatEventHandler
	{
		void OnBeat();
	}

	public interface IStrongBeatHandler : IBeatEventHandler
	{
		void OnStrongBeat();
	}
}
