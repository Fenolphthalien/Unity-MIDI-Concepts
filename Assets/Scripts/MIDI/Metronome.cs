using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityMIDI
{
	public class Metronome : BaseBeatEventDispatcher
	{
		[SerializeField]
		TimeSignature m_timeSignature = TimeSignature.Common();
		[SerializeField]
		int m_tempo = 120;

		int m_beatsElapsed, m_strongBeatsElapsed;
		float m_timeElapsed, m_totalTimeElapsed;

        float m_bps = 120f / 60, m_beatInterval = 1f/(120f / 60); // 1f / bps(tempo/60);

		public bool autoPlayOnStart;

        bool inBeatOffset, lastInBeatOffset;

        public BeatOffset beatOffset = new BeatOffset(100,BeatOffset.OffsetBy.MLLISECONDS);

		bool m_playing;

		void Start()
		{
			CalculateTempo ();
			if(autoPlayOnStart)
			{
				Play();
			}
			m_timeSignature = TimeSignature.Common ();
		}

		void Update () 
		{
            lastInBeatOffset = inBeatOffset;

			if(m_playing)
			{
                inBeatOffset = IsInBeatOffset();
				if(m_timeElapsed >= m_beatInterval)
				{
					if(m_beatsElapsed % m_timeSignature.beatsPerBar == 0)
					{
						m_strongBeatsElapsed++;
						SendStrongBeat();
					}
					m_beatsElapsed++;
					SendOnBeat();
					m_timeElapsed -= m_beatInterval;
				}
				m_timeElapsed += Time.deltaTime;
                m_totalTimeElapsed += Time.deltaTime;
			}
		}

		void CalculateTempo()
		{
            m_bps = (float)m_tempo / 60;
			m_beatInterval = 1f / (m_bps);
		}

		public void Play()
		{
			m_playing = true;
		}

		public void Stop(bool reset)
		{
			m_playing = false;
			if (reset)
				Reset ();
		}

		public void Reset()
		{
			m_strongBeatsElapsed = 0;
			m_beatsElapsed = 0;
			m_timeElapsed = 0;
		}

		public void SetTempo(int bpm)
		{
			m_tempo = bpm;
			CalculateTempo ();
		}

        public void IncrementTempo(int by)
        {
            m_tempo += by;
            CalculateTempo();
        }

        public void MultiplyTempo(int by)
        {
            m_tempo *= by;
            CalculateTempo();
        }

		public void Reset(int tempo, TimeSignature timeSignature)
		{
            m_beatsElapsed = 0;
            m_strongBeatsElapsed = 0;
            m_totalTimeElapsed = 0;
            m_timeElapsed = 0;
			SetTempo (tempo);
			m_timeSignature = timeSignature;
			Reset ();
		}

		public void AddSubscriber(IBeatEventHandler handler)
		{
			m_subscribers.Add (handler);
		}

		public void RemoveSubscriber(IBeatEventHandler handler)
		{
			m_subscribers.Remove (handler);
		}

        public bool IsInBeatOffset()
        {
            if (!m_playing)
                return false;
            int checkBeat = Mathf.RoundToInt(m_totalTimeElapsed / m_beatInterval);
            //Debug.Log(checkBeat);
            float checkTime = m_totalTimeElapsed - (checkBeat * m_beatInterval);
            //Debug.Log(checkTime);
            
            float offset = beatOffset.offsetBy == BeatOffset.OffsetBy.MLLISECONDS ? (beatOffset.offsetWidth*0.001f)*0.5f 
                : m_beatInterval * ((beatOffset.offsetWidth*100)*0.5f);
            bool inOffset = checkTime >= -offset && checkTime <= offset;
           
            if (inOffset)
                return true;
            return false;
        }

        public bool EnteredBeat()
        {
            return inBeatOffset && !lastInBeatOffset;
        }

        public bool ExitedBeat()
        {
            return !inBeatOffset && lastInBeatOffset;
        }

        public float ScaleBeatElapsed()
        {
            return m_timeElapsed / m_beatInterval;
        }

        public float ScaleBeatElapsed(float offset)
        {
            return (m_timeElapsed + (m_beatInterval * offset)) / m_beatInterval;
        }

        [System.Serializable]
        public struct BeatOffset
        {
            public enum OffsetBy { MLLISECONDS, PERCENTAGE };
            public OffsetBy offsetBy;
            public int offsetWidth;
            
            public BeatOffset(int _offsetWidth, OffsetBy _offsetBy)
            {
                offsetBy = _offsetBy;
                offsetWidth = _offsetWidth;
            }
        }
	}
}
