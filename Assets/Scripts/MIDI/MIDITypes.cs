using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityMIDI
{
	//Taken from the HMIDIIN type def.
	public enum InputStreamEvent
	{
		MM_MIM_OPEN =  0x3C1, 
		MM_MIM_CLOSE =  0x3C2, 
		MM_MIM_DATA =  0x3C3, 
		MM_MIM_LONGDATA = 0x3C4, 
		MM_MIM_ERROR   =  0x3C5,
		MM_MIM_LONGERROR = 0x3C6
	};
	
	public enum MIDIEvent{UNKNOWN = -1, META_SET_TEMPO = 0, META_TRACK_FINISHED = 1, NOTE_OFF = 128, NOTE_ON = 144, AFTERTOUCH =  160, CONTROL_CHANGE  = 176};

	public enum SendEventTo {SELF_OBJECT, SELF_AND_CHILD_OBJECTS};

	public enum Tone {C,Db,D,Eb,E,F,Gb,G,Ab,A,Bb,B,Count};	

	public enum ControlCode {MODULATION = 1, CHANNEL_VOLUME = 7};

    public enum TimeStep { PPQN = 0, SMPTE = 0x8000, OTHER = -1 };
	
	public enum Accidental{FLAT, SHARP};

	public enum ESetTempo {MICROSECS, BPM};

	public enum PlayBackSpeed {NORMAL = 256,HALF = 512,QUARTER = 1024,DOUBLE = 128}; //Stored this way because enums cannot be floating point. Convert using the ToFloat extension.

	public enum Metre {DUPLE = 4, TUPLE = 8};

    public enum Scale {MAJOR = 0, DORIAN = 2, PHRYGIAN = 4, LYDIAN = 5, MIXOLYDIAN = 7, MINOR = 9, LOCRIAN = 11, ATONAL};

	[System.Serializable]
	public struct MIDIMessage
    {
		public MIDIMessage(MIDIEvent e){m_midiEvent = e; channel = 0; keyEvent = new KeyEvent();wait = 0;}
		public MIDIMessage(MIDIEvent e, int c, KeyEvent _keyEvent){m_midiEvent = e; channel = c; keyEvent = _keyEvent;wait = 0;}
		public MIDIMessage(MIDIEvent e, int c, KeyEvent _keyEvent, uint _wait){m_midiEvent = (MIDIEvent)e; channel = c; keyEvent = _keyEvent; wait = _wait;}

		[SerializeField]
		MIDIEvent m_midiEvent;
		public MIDIEvent midiEvent {get{return m_midiEvent;}}

		public int channel;

		public uint wait;

		public KeyEvent keyEvent;

		public override string ToString ()
		{
			return string.Format ("MidiEvent {0}, {1}, channel {2}, wait {3}",midiEvent, keyEvent, channel, wait);
		}

		public int GetVelocity()
		{
			return keyEvent.velocity;
		}

        public void SetVelocity(int value)
        {
            int trueValue = (value <= 127 ? value : 127) >= 0 ? value : 0; //0 <= value <= 127
            keyEvent.velocity = trueValue;
        }

		public Tone GetNote()
		{
			return (Tone)keyEvent.note;
		}

		public int GetOctave()
		{
			return keyEvent.octave;
		}

		public bool IsNoteOn()
		{
			return midiEvent == MIDIEvent.NOTE_ON && keyEvent.velocity > 0;
		}

		public bool IsNoteOff()
		{
			return midiEvent == MIDIEvent.NOTE_OFF || midiEvent == MIDIEvent.NOTE_ON && keyEvent.velocity <= 0;
		}

        public bool IsAfterTouch()
        {
            return midiEvent == MIDIEvent.UNKNOWN;
        }
	}

	[System.Serializable]
	public struct KeyEvent
    {
		public int note;
		public int octave;
		public int velocity;
		
		public KeyEvent(int n, int o, int v){note = n; octave = o; velocity = v;}

		public override string ToString ()
		{
			return string.Format ("Note: {0}{1} at {2}",note.NoteToString(),octave,velocity);
		}

        public int ToInt()
        {
            return octave * 12 + note;
        }
	}

	[System.Serializable]
	public struct MIDIHeader 
    {

		[HideInInspector]
		public byte[] chunkType;
		[HideInInspector]
		public ulong length;
        [HideInInspector]
        public ushort format, tracks, division, ppqn;

		[HideInInspector]
		public TimeStep timeStepType;
	}

	[System.Serializable]
	public struct TimeSignature
	{
		[SerializeField]
		int m_beatsPerBar;
		public int beatsPerBar {get{return m_beatsPerBar;}}
		[SerializeField]
		Metre m_metre;
		public Metre metre{get{return m_metre;}}

		public TimeSignature(int _beatsPerBar, Metre _metre)
		{
			m_beatsPerBar = _beatsPerBar;
			m_metre = _metre;
		}

		public static TimeSignature Common()
		{
			return new TimeSignature (4, Metre.DUPLE);
		}

		public override string ToString ()
		{
			return string.Format ("{0}/{1}",m_beatsPerBar,(int)m_metre);
		}
	}

    [System.Serializable]
    public struct KeySignature 
    {
        public readonly Tone tonalCenter;
        public readonly Scale scale;

        public KeySignature(Tone key, Scale _scale) 
        {
            tonalCenter = key;
            scale = _scale;
        }
    }

	[System.Serializable]
	public class MIDITrack
	{ 
		[SerializeField]
		List<MIDIMessage> m_messages;
		public ReadOnlyCollection<MIDIMessage> messages{get{return m_messages.AsReadOnly();}}

        public string name;

		public Instrument instrument;

		[SerializeField]
		float m_seconds; //Length in seconds.
		public float seconds{get{return m_seconds;}}
        public int secondsRoundedUp { get { return Mathf.CeilToInt(m_seconds); } }
        public int secondsRoundedDown { get { return Mathf.FloorToInt(m_seconds); } }
        public bool lengthNonZero { get { return seconds > 0; } }

        public void SetVelocity(int message, int velocity)
        {
            m_messages[message].SetVelocity(velocity);
        }

		public MIDITrack(List<MIDIMessage> messageList, float trackSeconds)
		{
			m_messages = messageList;
			m_seconds = trackSeconds;
		}
	}
}