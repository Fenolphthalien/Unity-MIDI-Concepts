using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityMIDI.Import
{
	public static class MIDIImporter 
	{
		//----------------------------------------------------------------
		//	Load Asset
		//----------------------------------------------------------------
		public static void LoadIntoMIDIAsset(MIDI midi)
		{
			int bytesRead = 0;
			FileStream fStream;
			if (midi.usingRelativePath) {
				fStream = new FileStream (UnityMIDIPreferencesEditor.SourceFolderPath + midi.sourcePath + ".mid", FileMode.Open, FileAccess.Read);
			}
			else{
				fStream = new FileStream (midi.sourcePath + ".mid", FileMode.Open, FileAccess.Read);
			}
			midi.InitialiseHeader(fStream, ref bytesRead);
			ReadMIDI (ref bytesRead, midi, fStream);
		}
		
		
		static void ReadMIDI(ref int bytesRead, MIDI midi, FileStream fStream)
        {
			
			if(midi == null){
				Debug.LogError("Midi not initialised");
				return;
			}
			
			int tracksRead = 0;
			ulong trackLength = 0;
			byte[] longBuffer = new byte[4];
			//Data buffer;
			byte[] data = new byte[44100];
			
			if(midi.tracks.Count > 0)
            {
				midi.tracks.Clear();
			}
			
			midi.SetTempo (120, ESetTempo.BPM);
			
			while (tracksRead < midi.header.tracks)
			{
				
                //Read track chunk Header
				fStream.Read(longBuffer,0,4);
				string TrackHeaderChunk = longBuffer.String();

                //Track chink Check
				if(TrackHeaderChunk != "MTrk")
                {
					Debug.LogWarning("Track not valid.");
					return;
				}
				
				longBuffer.Initialize();
				
				fStream.Read(longBuffer,0, 4);
				trackLength = longBuffer.ULong();
				if (trackLength < 1)
                {
					Debug.LogWarning("Track length less than 1");
					return;
				}
				longBuffer.Initialize();

				if((int)trackLength > data.Length)
                {
					data = new byte[(int)trackLength];
				}
				fStream.Read(data,0, (int)trackLength);
				
				//Debug.Log("Track read");
				MIDITrack track =  ParseIntoTrack(ref data, trackLength, midi);
				
				//if(midi.header.format != 1 || (midi.header.format == 1 && tracksRead != 0)){ //In format 1 track 01 only contains meta data.
				if(track != null)
					midi.tracks.Add(track);

				//}

				//Prepare for next iteration.
				data.Initialize();
				tracksRead++;
			}
			float maxSeconds = 0;
			for(int i = 0; i < midi.tracks.Count; i++)
			{
				maxSeconds = midi.tracks[i].seconds > maxSeconds ? midi.tracks[i].seconds : maxSeconds;
			}
			midi.SetTrackLength (maxSeconds);
		}
		
		static MIDITrack ParseIntoTrack(ref byte[] data, ulong length, MIDI midi)
		{	
			uint deltaT = 0, totalTime = 0;
			ulong bytesRead = 0, microSecs = 500000, end = 0;
			bool bMetaHandled = false;
			string sStream = "";

			List<MIDIMessage> messageList = new List<MIDIMessage>();

			//Saved for running status.
			int channel = 0;
			byte lastEvent = 0; 
			uint lDeltaT = 0; //LastDeltaTime
			
			while (bytesRead < length)
			{
                lDeltaT = deltaT;
                totalTime += deltaT;
                deltaT = 0;

                HandleDeltaTime(ref data, ref bytesRead, ref deltaT);
                
                if (IsStatusByte(lastEvent) && IsDataByte((byte)data[bytesRead]) && IsDataByte((byte)data[bytesRead + 1])) // Running Status check;
				{
					switch(lastEvent)
					{
					case 0x80:
						HandleNoteOff(ref data,messageList,ref bytesRead,ref deltaT,ref totalTime, ref lDeltaT, ref channel, true);
						break;
					case 0x90:
						if((byte)data[bytesRead + 1] > 0)
                            HandleNoteOn(ref data, messageList, ref bytesRead, ref deltaT, ref totalTime, ref lDeltaT, ref channel, true);
						else //MIDI assumes a note off event if velocity is 0;
                            HandleNoteOff(ref data, messageList, ref bytesRead, ref deltaT, ref totalTime, ref lDeltaT, ref channel, true);
						break;
					case 0xB0:
                        HandleControlChange(ref data, messageList, ref bytesRead, ref deltaT, ref totalTime, ref lDeltaT, ref channel, true);
						break;
					default:
						break;
					}
				}
				//Note off
				else if(data[bytesRead] >= 0x80 && data[bytesRead] < 0x90)
				{
					lastEvent = 0x80;
					HandleNoteOff(ref data,messageList,ref bytesRead, ref deltaT,ref totalTime,ref lDeltaT, ref channel, false);
				}
				//Note on
				else if (data[bytesRead] >= 0x90 && data[bytesRead] < 0xA0)
				{
					lastEvent = 0x90;
					HandleNoteOn(ref data,messageList,ref bytesRead, ref deltaT,ref totalTime,ref lDeltaT, ref channel, false);
				}
				//Control change
				else if(data[bytesRead] >= 0xB0 && data[bytesRead] < 0xC0)
				{
					lastEvent = 0xB0;
					HandleControlChange(ref data,messageList,ref bytesRead, ref deltaT,ref totalTime,ref lDeltaT, ref channel, false);
				}

				//Meta events
				else if ((byte)data[bytesRead] == 0xFF)
                {

					Debug.Log("Meta event");
					bMetaHandled = false;

                    const uint readMetaDataOffset = 3;

					sStream = "";
					end = (byte)data[bytesRead + 2] + bytesRead; 

					switch (data[bytesRead + 1])
					{
						//Copyright
						case 0x02:
							Debug.Log("Copyright");
							bMetaHandled = true;
                            bytesRead += readMetaDataOffset;
                            end += readMetaDataOffset;
							while (bytesRead < end)
							{
								sStream += (char)data[bytesRead];
								bytesRead++;
							}
							midi.copyright = sStream; 
							sStream = "Copyright: " + sStream;
							break;

						//Track name
						case 0x03:
							Debug.Log("Track name");
							bMetaHandled = true;
                            bytesRead += readMetaDataOffset;
                            end += readMetaDataOffset;
							while (bytesRead < end)
							{
								sStream += (char)data[bytesRead];
								bytesRead++;
							}
							sStream = "Track Name: " + sStream;
							break;
						case 0x04:
							sStream +="Instrument name: ";
							break;
						case 0x05:
							sStream += "Lyrics: ";
							break;
						case 0x06:
							sStream += "Marker Text: ";
							break;
						case 0x07:
							sStream += "Cue point: ";
							break;
						case 0x20:
							sStream += "MIDI channel prefix assignment: ";
							break;
						case 0x2f:
							MIDIMessage message = new MIDIMessage(MIDIEvent.META_TRACK_FINISHED,0,new KeyEvent(),deltaT);
							messageList.Add(message);
							sStream +="End of Track.";
							break;
						case 0x51:
							Debug.Log("Change Tempo");
							bMetaHandled = true;
                            bytesRead += readMetaDataOffset;
							microSecs = ((ulong)(((byte)data[bytesRead] << 16) | ((byte)data[bytesRead + 1] << 8) | (byte)data[bytesRead + 2]));
							midi.SetTempo(microSecs, ESetTempo.MICROSECS);
                            bytesRead += readMetaDataOffset;
							break;
						case 0x58:
							sStream += "Time signature: ";
							break;
						case 0x59:
							sStream += "Key signature: ";
							break;
						case 0x7f:
							sStream += "Sequencer specific event: ";
							break;
						default:
							sStream += "Unreconised Metadata";
							break;
					}
					if (!bMetaHandled)
					{
						Debug.LogWarning(string.Format("Unhandled meta data at byte {0}.",bytesRead));

						bytesRead += 3;
						end += 3;

						while (bytesRead < end)
						{
							Debug.LogWarning("Skipping byte "+ bytesRead);
							bytesRead++;
						}
					}
				} 

				//Error handling
				else
				{
					Debug.LogWarning(string.Format("Fallen through at: {0}, Last Byte: {1} This Byte: {2}, Next Byte: {3} " ,bytesRead.ToString(),data[bytesRead-1].ToString("X"),data[bytesRead].ToString("X"),data[bytesRead + 1].ToString("X")));
					HandleFallthrough(ref data, ref bytesRead);
				}
			}
			return new MIDITrack (messageList,totalTime*midi.TDPS);
		}

		static void HandleNoteOff(ref byte[] data, System.Collections.Generic.List<MIDIMessage> messageList, ref ulong bytesRead, ref uint delta, ref uint totalTime, ref uint lDelta, ref int channel,bool handleAsRunningStatus)
		{
			KeyEvent note;
			MIDIMessage message;
			if (!handleAsRunningStatus) 
			{
				channel = (int)(data [bytesRead] - 0x80);
				note = MIDI.BuildNote (data [bytesRead + 1], data [bytesRead + 2]);
				message = new MIDIMessage (MIDIEvent.NOTE_OFF, channel, note, delta);
			
				messageList.Add (message);
				bytesRead += 3;
			}
			else
			{
				note = MIDI.BuildNote (data [bytesRead], data [bytesRead + 1]);
				message = new MIDIMessage (MIDIEvent.NOTE_OFF, channel, note, delta);
				messageList.Add (message);
				bytesRead += 2;
			}
		}

		static void HandleNoteOn(ref byte[] data, System.Collections.Generic.List<MIDIMessage> messageList, ref ulong bytesRead, ref uint delta, ref uint totalTime, ref uint lDelta, ref int channel,bool handleAsRunningStatus)
		{
			KeyEvent note;
			MIDIMessage message;
			if (!handleAsRunningStatus) 
			{
				channel = (int)(data [bytesRead] - 0x90);
				note = MIDI.BuildNote (data [bytesRead + 1], data [bytesRead + 2]);
				message = new MIDIMessage (MIDIEvent.NOTE_ON, channel, note, delta);
				
				messageList.Add (message);
				bytesRead += 3;
			}
			else
			{
				note = MIDI.BuildNote (data [bytesRead], data [bytesRead + 1]);
				message = new MIDIMessage (MIDIEvent.NOTE_ON, channel, note, delta);
				messageList.Add (message);
				bytesRead += 2;
			}
		}

		static void HandleControlChange(ref byte[] data, System.Collections.Generic.List<MIDIMessage> messageList, ref ulong bytesRead, ref uint delta,ref uint totalTime, ref uint lDelta, ref int channel,bool handleAsRunningStatus)
		{
			KeyEvent note;
			MIDIMessage message;
			if (!handleAsRunningStatus)
            {
				channel = (int)(data [bytesRead] - 0xB0);
				note = MIDI.BuildNote (data [bytesRead + 1], data [bytesRead + 2]);
				message = new MIDIMessage (MIDIEvent.CONTROL_CHANGE, channel, note, delta);
				
				messageList.Add (message);
				bytesRead += 3;
			} 
            else 
            {
				note = MIDI.BuildNote (data [bytesRead], data [bytesRead + 1]);
				message = new MIDIMessage (MIDIEvent.CONTROL_CHANGE, channel, note, lDelta);
				messageList.Add (message);
				bytesRead += 2;
			}
		}

		static void HandleFallthrough(ref byte[] data, ref ulong bytesRead )
		{
			bytesRead++;
		}

        /* http://www.gamedev.net/topic/567104-midi-file-how-do-delta-times-differ-from-running-status-data/ */
        // Apparently Delta times are handled the same regardless of running status.
        static void HandleDeltaTime(ref byte[] data, ref ulong bytesRead, ref uint deltaTime)
        {
            while((byte)data[bytesRead] >= 0x80) // HandleTimestep.
            {
                deltaTime += ((uint)(data[bytesRead] - 0x80)) * 128;
                bytesRead++;
            }
            deltaTime += (uint)(data[bytesRead]);
            bytesRead++;
            //Assume bytes after this are status bytes or databytes.
        }

        static bool IsStatusByte(byte b)
        {
            return b >= 0x80 && b <= 0xE0;
        }

        static bool IsDataByte(byte b)
        {
            return b >= 0 && b <= 127;
        }
	}
}
