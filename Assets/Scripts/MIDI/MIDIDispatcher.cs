using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityMIDI
{
	public abstract class MIDIDispatcher : MonoBehaviour 
	{

		//Event delegate definition and callers - Faster than messages.
		protected delegate void ControlDelegate(MIDIMessage message);
		protected ControlDelegate m_NoteOn, m_NoteOff, m_Aftertouch;

		[SerializeField]
		protected IMIDIEventHandler[] m_subscriber;

		protected virtual void OnEnable()
		{
			m_subscriber = SubscribeMIDIHandlers(GetComponents<IMIDIEventHandler> ());
		}
		
		protected virtual void OnDisable()
		{
			m_NoteOn = null;
			m_NoteOff = null;
			m_subscriber = null;
		}

		protected IMIDIEventHandler[] SubscribeMIDIHandlers(IMIDIEventHandler[] handlers)
		{
			if (handlers == null || handlers.Length < 0)
				return null;

			for(int i = 0; i < handlers.Length; i++)
			{
                //Prevent subscribing a reference to self if a child class implements any MIDI interfaces.
                MIDIDispatcher selfCheckObj = handlers[i] as MIDIDispatcher; 
                if (selfCheckObj != null && selfCheckObj == this)
                    continue;

				//Subscribe note on interface
				if(handlers[i] is INoteOnHandler)
					m_NoteOn += ((INoteOnHandler)handlers[i]).OnNoteOn;

				//Subscribe note off interface
				if(handlers[i] is INoteOffHandler)
					m_NoteOff += ((INoteOffHandler)handlers[i]).OnNoteOff;

                //Subscribe aftertouch interface
                if (handlers[i] is IAftertouchHandler)
                    m_Aftertouch += ((IAftertouchHandler)handlers[i]).OnAftertouch;
			}
			return handlers;
		}

        protected bool Dispatch(MIDIMessage message)
        {
            if (m_subscriber == null)
                return false;
            if (message.IsNoteOn() && m_NoteOn != null)
            {
                m_NoteOn.Invoke(message);
                return true;
            }
            else if (message.IsNoteOff() && m_NoteOff != null)
            {
                m_NoteOff.Invoke(message);
                return true;
            }
            return false;
        }
	}
}
