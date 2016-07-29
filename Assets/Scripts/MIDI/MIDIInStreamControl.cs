using UnityEngine;
using System.Collections.Generic;
using UnityMIDI.Stream;
using System.Collections.ObjectModel;

namespace UnityMIDI
{

	public class MIDIInStreamControl : MIDIDispatcher
	{
		MIDIInStream midiInStream;

		MIDIMessage[] m_lastMessages;

		public Instrument instrument;

		[SerializeField]
		protected int m_targetDeviceIndex;

		void Awake()
		{
			midiInStream = MIDIInStream.PrepareStream(m_targetDeviceIndex);
			return;
		}

		void OnDestroy()
		{
			CloseStream ();
		}

		void Update()
		{
			if(midiInStream != null)
			{
				MIDIMessage[] messages = midiInStream.GetStreamedMessages ();
				HandleMessages (messages);
			}
		}

		//Force the stream to close before the application does to prevent an application crash and potential memory leak.
		void OnApplicationQuit() 
		{
			CloseStream ();
		}

		void HandleMessages(MIDIMessage[] messages)
		{
			if (messages == null)
				return;
			for(int i = 0; i < messages.Length; i++)
			{
                Dispatch(messages[i]);
			}
            m_lastMessages = messages;
		}

		public MIDIMessage[] GetLastMessages()
		{
			return m_lastMessages;
		}

		public void CloseStream()
		{
			if(midiInStream != null)
			{
				midiInStream.CloseStream();
			}
		}
	}
}
