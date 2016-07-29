using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace UnityMIDI
{
	//This is deprecated I think. Code has been moved to MIDIIStream.cs
	public class MIDIListener : MIDIBroadcaster {

		//====================================================
		// DLL functions.
		//====================================================
		[DllImport("UnityMidi", EntryPoint="GetInputDevices")]
		static extern int GetNumInDevices();

		[DllImport("UnityMidi",EntryPoint = "Test")]
		static extern int Test ();

		[DllImport("UnityMidi", EntryPoint = "Open")]
		static extern int Open (ref IntPtr MidiHandle, MIDIInProc midiIn);

		[DllImport("UnityMidi", EntryPoint = "Close")]
		static extern void Close (ref IntPtr MidiHandle);

		static bool bRunning = false;

		private static IntPtr MidiHandle;

		delegate void MIDIInProc(IntPtr hMidiIn, int wMsg,uint dwInstance,uint dwParam01,uint dwParam02);
		static MIDIInProc MIDIIn;

		static List<MIDIMessage> MidiInQueue = new List<UnityMIDI.MIDIMessage>();
		static Dictionary<int, MIDIListener> Subscribers = new Dictionary<int, MIDIListener> ();

		public SendEventTo sendEventsTo = SendEventTo.SELF_OBJECT;

		public bool bDebug;

		AudioClip[] soundbank = new AudioClip[25];

		public bool usingSoundBank;

		public string SoundBankPath = "";

		string kSoundBankPath = "Audio/Samples/";

		AudioSource audioSource;

		const int NOTE_OFF = 128, NOTE_ON = 144, AFTERTOUCH = 160, CONTROL_CHANGE = 176, PROGRAM_CHANGE = 192;

		private int ID;
		static int NextID = 0;

		void Awake () 
		{
			if(usingSoundBank){
				audioSource = GetComponent<AudioSource>();
				if(audioSource != null){
					int octave = 3, pitch = 0, i = 0;
					while(i < 25){
						Debug.Log(kSoundBankPath + SoundBankPath + "_" + octave.ToString() + "_" + pitch.ToString());
						soundbank[i] = (AudioClip)Resources.Load(kSoundBankPath + SoundBankPath + "_"+octave.ToString()+"_"+pitch.ToString());
						if(pitch >= 11){
							pitch -= 12;
							octave++;
						}
						pitch++;
						i++;
					}
				}
			}
		}

		void OnApplicationQuit(){
			ForceMidiClose ();
		}

		void OnEnable(){
			SubscribeToMidi ();
		}

		void OnDisable(){
			UnsubscribeToMidi ();
		}

		void Update (){
			if(MidiInQueue.Count > 0 && Subscribers.Count > 0){
				for(int i = 0;i < MidiInQueue.Count; i++ ){
					for(int n = 0; n < Subscribers.Count; n++){
						if(Subscribers[n].sendEventsTo == SendEventTo.SELF_AND_CHILD_OBJECTS)
							MIDI.BroadcastMIDIMessage(Subscribers[n],MidiInQueue[i]);
						else
							MIDI.SendMIDIMessage(Subscribers[n],MidiInQueue[i]);
					}
				}
				MidiInQueue.Clear();
			}
		}

		void SubscribeToMidi(){
			MIDIIn += midiInProc;
			Subscribers.Add (NextID, this);
			ID = NextID;
			NextID++;
			Debug.Log("Subscriber Added");
			if(!bRunning){
				if(MidiHandle == null)
					MidiHandle = new IntPtr();
				switch(Open(ref MidiHandle, MIDIIn)){
				case 0:
					bRunning = true;
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
		}

		void UnsubscribeToMidi(){
			Subscribers.Remove (ID);
			MIDIIn -= midiInProc;
			if (bRunning && Subscribers.Count < 1) {
				Close (ref MidiHandle);
				bRunning = false;
			}
		}

		void ForceMidiClose(){
			MIDIIn -= midiInProc;
			if (bRunning) {
				Close (ref MidiHandle);
				bRunning = false;
			}
			Subscribers.Clear ();
		}

		//Callback to MIDI handle when an event is recieved - Any non static variable or function calls within this will cause this to fail.
		void midiInProc(IntPtr hMidiIn, int wMsg,uint dwInstance,uint dwParam01,uint dwParam02){
			switch (wMsg) {
			case (int)InputStreamEvent.MM_MIM_OPEN:
					Debug.Log("Midi Opened");
				break;
			case (int)InputStreamEvent.MM_MIM_CLOSE:
					Debug.Log("Midi Closed");
				break;
			case (int)InputStreamEvent.MM_MIM_DATA:
				//Debug.Log("Midi Event");
				BuildMessage(dwParam01);
				//Debug.Log("Event Added");
				break;
			case (int)InputStreamEvent.MM_MIM_LONGDATA:
				break;
			case (int)InputStreamEvent.MM_MIM_ERROR:
				Debug.LogWarning("Midi Error");
				break;
			case (int)InputStreamEvent.MM_MIM_LONGERROR:
				Debug.LogWarning("Midi Long Error");
				break;
			default:
				break;
			}
			return;
		}

		//#define NOTE_OFF 128

		//#define Note_On 144

		//#define AFTERTOUCH 160
		
		//#define CONTROL_CHANGE 176
		
		//#define PROGRAM_CHANGE 192

		void BuildMessage(uint dwParam01){
			UInt16 loWord, HiWord;
			byte Hihi, Hilo, Lohi, Lolo;
			TypeUtility.SplitDoubleWord (dwParam01, out HiWord, out loWord);
			TypeUtility.SplitWord (HiWord, out Hihi, out Hilo);
			TypeUtility.SplitWord (loWord, out Lohi, out Lolo);

			int mEvent = 0, Channel = 0;
			if (Lolo < 144) {
				Channel = Lolo % 128;
				mEvent = Lolo - Channel;
			}
			else if (Lolo < 160) {
				Channel = Lolo % 144;
				mEvent = Lolo - Channel;
			}
			else if (Lolo < 176) {
				Channel = Lolo % 160;
				mEvent = Lolo - Channel;
			}
			MidiInQueue.Add (new MIDIMessage ((MIDIEvent)mEvent, Channel, MIDI.BuildNote (Lohi, Hilo)));
		}

		void OnNoteOn(MIDIMessage message){
			int i = (int)message.keyEvent.note + (message.keyEvent.octave - 3) * 12;
			if (usingSoundBank) {
				if(audioSource.isPlaying)
					audioSource.Stop();
				audioSource.clip = soundbank [i];
				audioSource.Play ();
			}
		}

		void OnNoteOff(MIDIMessage message)
		{
			if (usingSoundBank) 
			{
				if(audioSource.isPlaying)
					audioSource.Stop();
			}
		}	
	}
}
