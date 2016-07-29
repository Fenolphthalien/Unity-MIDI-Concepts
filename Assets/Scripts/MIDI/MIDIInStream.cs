using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UnityMIDI.Stream
{
	public class MIDIInStream : IDisposable
	{
		//====================================================
		// DLL functions.
		//====================================================
		[DllImport("UnityMidi", EntryPoint="GetInputDevices")]
		static extern int GetNumInDevices();
		
		[DllImport("UnityMidi",EntryPoint = "Test")]
		static extern int Test ();
		
		[DllImport("UnityMidi", EntryPoint = "Open")]
		static extern int Open (ref IntPtr MidiHandle, MIDIInProc midiIn);

        [DllImport("UnityMidi", EntryPoint = "Open")]
        static extern int OpenDevice(ref IntPtr MidiHandle, MIDIInProc midiIn);

		delegate void MIDIInProc(IntPtr hMidiIn, int wMsg,uint dwInstance,uint dwParam01,uint dwParam02);
		static MIDIInProc MIDIIn;
		
		[DllImport("UnityMidi", EntryPoint = "Close")]
		static extern void Close (ref IntPtr MidiHandle);

		//====================================================
		// members.
		//====================================================
		static List<MIDIMessage> MidiInQueue = new List<UnityMIDI.MIDIMessage>();
		static IntPtr MidiHandle;

		public static MIDIInStream PrepareStream(int device)
		{
			int devices = GetNumInDevices ();
			if(device > -1 && devices > device)
			{
				Debug.Log("Creating new stream");
				return new MIDIInStream ();
			}
			return null;
		}
		
		public void CloseStream()
		{
			this.Dispose ();
		}

		MIDIInStream()
		{
			BeginStreaming ();
		}

		void BeginStreaming()
		{
			MIDIIn = midiInProc;
			if(MidiHandle == null)
				MidiHandle = new IntPtr();
			switch(Open(ref MidiHandle, MIDIIn))
            {
			case 0:
				break;
			case 1: 
				Debug.LogWarning("No output devices detected");
				break;
			case 2:
				Debug.LogError("No input devices detected");
				break;
			case 3:
				Debug.LogError("No output or input devices detected");
				break;
			default:
				break;
			}
		}

		~MIDIInStream()
		{
		}

		//Callback to MIDI handle when an event is recieved from the stream - Any non static variable or function calls(<-varify) within this will cause this to fail.
		void midiInProc(IntPtr hMidiIn, int wMsg,uint dwInstance,uint dwParam01,uint dwParam02){
			//Debug.Log("Message recieved");
			switch (wMsg) 
            {
			case (int)InputStreamEvent.MM_MIM_OPEN:
				break;
			case (int)InputStreamEvent.MM_MIM_CLOSE:
				break;
			case (int)InputStreamEvent.MM_MIM_DATA:
				BuildMessage(dwParam01);
				break;
			case (int)InputStreamEvent.MM_MIM_LONGDATA:
				break;
			case (int)InputStreamEvent.MM_MIM_ERROR:
                CloseStream();
				break;
			case (int)InputStreamEvent.MM_MIM_LONGERROR:
                CloseStream();
				break;
			default:
				break;
			}
			return;
		}

		void BuildMessage(uint dwParam01)
        {
			UInt16 loWord, HiWord;
			byte Hihi, Hilo, Lohi, Lolo;
			TypeUtility.SplitDoubleWord (dwParam01, out HiWord, out loWord);
			TypeUtility.SplitWord (HiWord, out Hihi, out Hilo);
			TypeUtility.SplitWord (loWord, out Lohi, out Lolo);
			
			int mEvent = 0, Channel = 0;
			if (Lolo < 144) 
            {
				Channel = Lolo % 128;
				mEvent = Lolo - Channel;
			}
			else if (Lolo < 160) 
            {
				Channel = Lolo % 144;
				mEvent = Lolo - Channel;
			}
			else if (Lolo < 176) 
            {
				Channel = Lolo % 160;
				mEvent = Lolo - Channel;
			}
			MidiInQueue.Add (new MIDIMessage ((MIDIEvent)mEvent, Channel, MIDI.BuildNote (Lohi, Hilo)));
		}

		public void Dispose()
		{
			Close (ref MidiHandle);
			MIDIIn = null;
		}

		public MIDIMessage[] GetMessages()
		{
			return MidiInQueue.ToArray ();
		}

		public MIDIMessage[] GetStreamedMessages()
		{
			MIDIMessage[] outObj =  MidiInQueue.ToArray();
			MidiInQueue.Clear ();
			return outObj;
		}
	}
}
