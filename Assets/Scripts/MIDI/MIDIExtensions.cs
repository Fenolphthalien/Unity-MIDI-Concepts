using UnityEngine;
using System.IO;
using System.Collections;

namespace UnityMIDI{

	public static class MIDIClassExtensions {

		static float byteDiv = 1f / 127; 
		static float kByteDenominator = 1.0f /256;
		
		public static float ToFloat(this PlayBackSpeed e)
		{
			return (float)e * kByteDenominator;
		}
		
		public static string NoteToString(this int n){
			switch (n){
			case 0:
				return "C";
			case 1:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "C#";
				else
					return "Db";
			case 2:
				return "D";
			case 3:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "D#";
				else
					return "Eb";
			case 4:
				return "E";
			case 5:
				return "F";
			case 6:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "F#";
				else
					return "Gb";
			case 7:
				return "G";
			case 8:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "G#";
				else
					return "Ab";
			case 9:
				return "A";
			case 10:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "A#";
				else
					return "Bb";
			case 11:
				return "B";
			default:
				return "error";
			}
		}
		
		public static string NoteToString(this Tone n){
			switch (n){
			case Tone.C:
				return "C";
			case Tone.Db:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "C#";
				else
					return "Db";
			case Tone.D:
				return "D";
			case Tone.Eb:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "D#";
				else
					return "Eb";
			case Tone.E:
				return "E";
			case Tone.F:
				return "F";
			case Tone.Gb:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "F#";
				else
					return "Gb";
			case Tone.G:
				return "G";
			case Tone.Ab:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "G#";
				else
					return "Ab";
			case Tone.A:
				return "A";
			case Tone.Bb:
				if(UnityMIDIPreferences.GetAccidental() == Accidental.SHARP)
					return "A#";
				else
					return "Bb";
			case Tone.B:
				return "B";
			default:
				return "error";
			}
		}

		public static string String(this KeyEvent t){
			return t.note.NoteToString () + t.octave.ToString() + " played at a velocity of " + t.velocity.ToString();
		}
		
		public static Color ToColor(this KeyEvent t)
		{
			return UnityMIDIPreferences.GetColor (t.note);
		}

		public static Color ToColor(this KeyEvent t, float volume)
		{
			Color c = UnityMIDIPreferences.GetColor (t.note);
			byte value = 1;
			if (((Tone)t.note).Black ())
				value = 0;
			return new Color (Mathf.Lerp (value, c.r, volume*byteDiv), Mathf.Lerp (value, c.g, volume*byteDiv), Mathf.Lerp (value, c.b, volume*byteDiv), 1);

		}

        public static Color TintByOctave(this Color colour, int octave, int middleOctave = 4)
        {
            int midOctave = middleOctave > 0 ? middleOctave : 4;
            float tint = (float)(octave) / midOctave;
            float r, g, b;
            r = colour.r * tint > 1 ? 1 : colour.r * tint;
            g = colour.g * tint > 1 ? 1 : colour.g * tint;
            b = colour.b * tint > 1 ? 1 : colour.b * tint;
            return new Color(r,g,b,colour.a);
        }
		
		public static bool Black(this Tone n)
		{
			return n == Tone.Db || n == Tone.Eb || n == Tone.Gb || n == Tone.Ab || n == Tone.Bb;
		}
		
		public static void Initialise(this MIDIHeader header, FileStream fStream, ref int bytesRead)
		{
			if(fStream == null || !fStream.CanRead){return;}
			
			header = new MIDIHeader ();
			
			header.chunkType = new byte[4];
			byte[] buffer = new byte[6];
			fStream.Read (header.chunkType,0,4);
			bytesRead += 4;
			
			//Debug.Log (header.chunkType.String ());
			
			fStream.Read(buffer,0,4);
			header.length = buffer.ULong();
			bytesRead += 4;
			
			buffer.Initialize ();
			//Debug.Log (header.length.ToString());
			
			fStream.Read(buffer,0,6);
			header.format = buffer.UShort (0);
			header.tracks = buffer.UShort (2);
			header.division = buffer.UShort (4);
			
			Debug.Log (header.format.ToString ());
			Debug.Log (header.tracks.ToString ());
			//Debug.Log (header.division.ToString ());
			
			/* http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte */
			header.timeStepType = (TimeStep)(header.division & (1 << 14));
			
			if (header.timeStepType != TimeStep.PPQN) {
				Debug.LogWarning("Invalid time step. Please use PPQN");
				return;
			}
			header.ppqn = (ushort)((header.division) & 0x7fff); // Get bits 0-14
			
			bytesRead += 6;
		}
		
		public static void InitialiseHeader(this MIDI midi, FileStream fStream, ref int bytesRead)
		{
			if(fStream == null || !fStream.CanRead){return;}
			
			midi.header = new MIDIHeader ();
			
			midi.header.chunkType = new byte[4];
			byte[] buffer = new byte[6];
			fStream.Read (midi.header.chunkType,0,4);
			bytesRead += 4;
			
			fStream.Read(buffer,0,4);
			midi.header.length = buffer.ULong();
			bytesRead += 4;
			
			buffer.Initialize ();
			
			fStream.Read(buffer,0,6);
			midi.header.format = buffer.UShort (0);
			midi.header.tracks = buffer.UShort (2);
			Debug.Log ("Number of tracks: " + buffer.UShort (2).ToString());
			midi.header.division = buffer.UShort (4);
			
			/* http://stackoverflow.com/questions/4854207/get-a-specific-bit-from-byte */
			midi.header.timeStepType = (TimeStep)(midi.header.division & (1 << 14)); // 2^15
            if (midi.header.timeStepType != 0)
            {
                if (midi.header.timeStepType == TimeStep.SMPTE) //Is SMPTE format.
                {
                    uint frameRate = (uint)(((short)midi.header.division >> 7) & 0x7f); //Shift division short right by 8 bits and set bit 7(was bit 15) to 0.
                    uint frameRes = (uint)((short)midi.header.division & 0xff); //Mask the last byte on the division short.
                    midi.header.ppqn = (ushort)(frameRate * frameRes);
                }
                else
                {
                    Debug.LogWarning("Invalid time step.");
                    return;
                }
            }
            else
            {
                midi.header.ppqn = (ushort)((midi.header.division) & 0x7fff); // Get bits 0-14
            }
			
			bytesRead += 6;
		}

		public static float DefaultVolume(this MIDIEvent m)
		{
			switch (m) {
			case MIDIEvent.NOTE_ON:
				return 1;
			default:
				return 0;
			}
		}
	}

	public static class MIDIRelatedExtensions
	{
		public static string String(this byte[] bytes){
			string s = "";
			for(int i = 0; i < bytes.Length; i++){
				s += (char)bytes[i];
			}
			return s;
		}

		public static float MicroSecondsToSeconds(this ulong i)
		{
			return (float)(i) * 0.000001f;
		}
		
		public static float MicroSecondsToSeconds(this uint i)
		{
			return (float)(i) * 0.000001f;
		}
		
		public static float MicroSecondsToSeconds(this float i)
		{
			return i * 0.000001f;
		}
		
		public static float MicroSecondsToSeconds(this double i)
		{
			return (float)(i * 0.000001f);
		}

		public static ulong ULong(this byte[] bytes)
		{
			ulong o = (ulong)((bytes [0] << 24) | (bytes [1] << 16) | (bytes [2] << 8) | (bytes [3]));
			return o;
		}
		
		public static ushort UShort(this byte[] bytes, int offset)
		{
			ushort o = (ushort)((bytes [offset] << 8) | (bytes [offset+1]));
			return o;
		}
		
		public static float UByteToFloat(this byte b)
		{
			return (float)(b) / 256;
		}
		
		public static float UByteToFloat(this int i)
		{
			return (float)(i) / 256;
		}
		
		public static float UByteToFloat(this uint i)
		{
			return (float)(i) / 256;
		}
		
		
		public static float ByteToFloat(this byte b)
		{
			return (float)(b) / 128;
		}
		
		public static float ByteToFloat(this int i)
		{
			return (float)(i) / 128;
		}
		
		public static float ByteToFloat(this uint i)
		{
			return (float)(i) / 128;
		}
	}
}
