using UnityEngine;
using UnityMIDI;
using System.Collections;

public class PianoKey : MIDIMaskedReciever, INoteOnHandler, INoteOffHandler
{
	public VelocityVisualiser velocityVisualiser;
	Material m;
	MIDIMessage lastMessage;

	void Awake () 
	{
		if (GetComponent<MeshRenderer> ()) {
			m = GetComponent<MeshRenderer> ().material;
			if(note.Black()){
				m.color = Color.black;
			}
			else {
				m.color = Color.white;
			}
			if(velocityVisualiser)
				velocityVisualiser.SetMaterial(m);
		}
	}

	public void OnNoteOn(MIDIMessage midiMessage)
	{
		if (note == (Tone)midiMessage.keyEvent.note && Octave == midiMessage.keyEvent.octave) {
			m.color = midiMessage.keyEvent.ToColor (midiMessage.keyEvent.velocity);
			if(velocityVisualiser){
				velocityVisualiser.Trigger(midiMessage.keyEvent.velocity);
			}
			lastMessage = midiMessage;
		}
	}

	public void OnNoteOff(MIDIMessage midiMessage)
	{
		if(velocityVisualiser != null)
		{
			velocityVisualiser.Release();
		}
		if (note == (Tone)midiMessage.keyEvent.note && Octave == midiMessage.keyEvent.octave) 
		{
			if (note.Black ())
				m.color = Color.black;
			else
				m.color = Color.white;
		}
	}

	public void OnAftertouch(MIDIMessage midiMessage)
	{
  	}
}