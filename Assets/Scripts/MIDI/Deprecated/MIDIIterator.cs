//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//
//namespace UnityMIDI{
//
//	public class MIDITrackIterator : UnityEngine.Object, IDisposable {
//
//		List<MIDIMessage> m_messages = new List<MIDIMessage>();
//		List<CachedMessage> m_messageCache = new List<CachedMessage> ();
//
//		MIDITrack m_midiTrack;
//
//		bool m_finished;
//
//		int m_iterator = 0;
//
//		public void Open(MIDITrack track)
//		{
//			if (track != m_midiTrack) {
//				m_midiTrack = track;
//				m_iterator = 0;
//				m_messages.Clear ();
//				m_messageCache.Clear ();
//				m_finished = true;
//				return;
//			}
//			Debug.LogWarning ("This iterator is already opened with this track, please use restart instead.");
//		}
//
//		public MIDIMessage[] GetMessages(out MIDIMessage lastMessage)
//		{
//			m_messages.Clear ();
//
//			MIDIMessage lm_next;
//			bool lm_handleThisFrame = true;
//			do {
//				bool lm_wait;
//				if(m_messageCache.Count - 1 < m_iterator)
//				{
//					lm_next = m_midiTrack.GetMessage (m_iterator);
//					m_messages.Add (lm_next);
//					lm_wait = lm_next.wait > 0;
//					m_messageCache.Add(new CachedMessage(lm_next,lm_wait));
//				}
//				else
//				{
//					lm_next = m_messageCache[m_iterator].GetMessage();
//					lm_wait = m_messageCache[m_iterator].GetWait();
//				}
//				m_iterator++;
//				if(m_iterator - 1 > m_midiTrack.Count())
//				{
//					m_finished = true;
//				}
//				if (lm_wait) {
//					lm_handleThisFrame = false;
//				}
//			} while(lm_handleThisFrame);
//			lastMessage = m_messages[m_messages.Count - 1];
//			return m_messages.ToArray ();
//		}
//
//		public void Restart()
//		{
//			m_iterator = 0;
//			m_finished = true;
//		}
//
//		public void Dispose()
//		{
//			m_messages = null;
//		}
//
//		public bool Finished()
//		{
//			return m_finished;
//		}
//
//		struct CachedMessage
//		{
//			MIDIMessage m_message;
//			bool m_wait;
//
//			public CachedMessage(MIDIMessage message, bool wait)
//			{
//				m_message = message;
//				m_wait = wait;
//			}
//
//			public MIDIMessage GetMessage()
//			{
//				return m_message;
//			}
//
//			public bool GetWait()
//			{
//				return m_wait;
//			}
//		}
//	}
//}