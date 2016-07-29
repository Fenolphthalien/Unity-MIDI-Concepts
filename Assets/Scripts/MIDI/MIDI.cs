using UnityEngine;
using System.Collections.Generic;

namespace UnityMIDI
{
	[System.Serializable]
	public class MIDI : ScriptableObject {

		//----------------------------------------------------------------
		//	Public fields
		//----------------------------------------------------------------

		[SerializeField]
		public string sourcePath = "";
		
		[SerializeField]
		public MIDIHeader header = new MIDIHeader();
		[SerializeField]
		public List<MIDITrack> tracks = new List<MIDITrack>();

        [SerializeField]
        public string displayName = "";
		
		[SerializeField]
		public string copyright = "";
		
		const int kMinuteMicroSecs = 60000000;
		
		[SerializeField]
		public ulong BPM = 120, microSecs = 5000000;
		
        [SerializeField]
		public float TDPS = 0; //TDPS = seconds per tick(SPNQ/PPQN)

        [SerializeField]
        public TimeSignature timeSignature = TimeSignature.Common(); 

		public bool initialised
		{
			get 
            {
				return tracks.Count > 0;
			}
		}

        public int trackCount { get { return tracks != null ? tracks.Count : -1; } }

		public bool usingRelativePath = true;

		public AudioClip audioClip;

		[SerializeField]
		float m_seconds; //The length of the longest track in seconds.
		public float seconds{get{ return m_seconds; }}
		public int secondsRoundedUp {get{return Mathf.CeilToInt(m_seconds);}}
		public int secondsRoundedDown{get{return Mathf.FloorToInt(m_seconds);}}
		public bool lengthNonZero{get{return seconds > 0;}}
		
		//----------------------------------------------------------------
		//	Public Methods
		//----------------------------------------------------------------

		public void SetTrackLength(float trackSeconds)
		{
			m_seconds = trackSeconds;
		}
		
		public void SetTempo(ulong value, ESetTempo t)
        {
			if (t == ESetTempo.MICROSECS) 
            {
				microSecs = value;
				BPM = kMinuteMicroSecs / value; // Calculate BPM.
			}
			else{
				BPM = value;
				microSecs = kMinuteMicroSecs / value; // Calculate microsecs.
			}
			//Debug.Log("Micro seconds: " + microSecs.ToString());
            float SPQN = GetSecondsPerQuarterNote();
			TDPS = SPQN / header.ppqn;
		}

		public ushort GetFormat()
		{
			return header.format;
		}

		public ushort GetDivision()
		{
			return header.division;
		}

		public ushort GetPulsesPerQuarternote()
		{
			return header.ppqn;
		}

        public float GetSecondsPerQuarterNote()
        {
            return microSecs.MicroSecondsToSeconds();
        }

		public ushort GetNumberOfTracksShort()
		{
			return header.tracks;
		}

        public float GetLengthRelativeToQuarterNote(MIDIMessage message)
        {
            float spnq = GetSecondsPerQuarterNote();
            return (message.wait * TDPS) / spnq;
        }

		public int GetNumberOfTracks()
		{
			return tracks.Count;
		}

        public MIDITrack GetTrack(int index)
        {
            if (index >= 0 && index < tracks.Count)
            {
                return tracks[index];
            }
            return null;
        }
		
		public static KeyEvent BuildNote(byte b, byte v){return new KeyEvent (b % 12,b/12 - 2,v);}
		
		public static void SendMIDIMessage(MIDIBroadcaster obj, MIDIMessage message)
		{
			switch(message.midiEvent){
			case MIDIEvent.NOTE_OFF:
				obj.SendMessage("OnNoteOff", message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.NOTE_ON:
				obj.SendMessage("OnNoteOn",message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.CONTROL_CHANGE:
				obj.SendMessage("OnControlChange",message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.META_TRACK_FINISHED:
				obj.SendMessage("OnTrackFinished", message, SendMessageOptions.DontRequireReceiver);
				break;
			default:
				break;
			}
		}
		
		public static void BroadcastMIDIMessage(MIDIBroadcaster obj, MIDIMessage message)
		{
			switch(message.midiEvent){
			case MIDIEvent.NOTE_OFF:
				obj.BroadcastMessage("OnNoteOff", message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.NOTE_ON:
				obj.BroadcastMessage("OnNoteOn",message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.CONTROL_CHANGE:
				obj.BroadcastMessage("OnControlChange",message,SendMessageOptions.DontRequireReceiver);
				break;
			case MIDIEvent.META_TRACK_FINISHED:
				obj.BroadcastMessage("OnTrackFinished", message, SendMessageOptions.DontRequireReceiver);
				break;
			default:
				break;
			}
		}

        public string GetName()
        {
            return displayName != null && displayName.Length > 0 ? displayName : name;
        }
	}
}

