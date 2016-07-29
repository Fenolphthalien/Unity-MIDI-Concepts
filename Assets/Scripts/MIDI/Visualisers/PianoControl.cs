using UnityEngine;
using System.Collections;
using UnityMIDI;

public class PianoControl : MonoBehaviour, INoteOnHandler, INoteOffHandler
{
	[SerializeField,HideInInspector]
	PianoKey[] m_pianoKey;

	public Tone startNote;
	public int startOctave;

	public void OnNoteOn(MIDIMessage message)
	{
		foreach(PianoKey key in m_pianoKey)
		{
			key.OnNoteOn(message);
		}
	}

	public void OnNoteOff(MIDIMessage message)
	{
		foreach(PianoKey key in m_pianoKey)
		{
			key.OnNoteOff(message);
		}
	}

	public int GetOctaves()
	{
		return m_pianoKey != null ? m_pianoKey.Length / 12 : 0; 
	}

	[ContextMenu("Assign keys from children")]
	void GetChildKeys()
	{
		PianoKey[] key = GetComponentsInChildren<PianoKey>();
		int oct = startOctave;
		Tone note;
		for(int i = 0; i < key.Length; i++)
		{
			note = (Tone)(((int)startNote + i) % (int)Tone.Count); 
			if(note == Tone.C && i > 0)
			{
				oct++;
			}
			key[i].note = note;
			key[i].Octave = oct;
		}
		m_pianoKey = key;
	}
}
