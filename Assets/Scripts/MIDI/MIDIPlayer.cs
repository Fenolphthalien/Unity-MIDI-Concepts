using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;

namespace UnityMIDI
{
	public class MIDIPlayer : MIDIDispatcher 
    {
        //=============================================
        // Members - Public
        //=============================================
		public bool autoPlay = true, playAudio = true;
        //=============================================
        // Members - Private
        //=============================================
        [SerializeField]
        MIDI m_midi;

		[SerializeField]
		float m_delayFor = 2;

		bool m_playing = false, m_lastPlaying, m_break;

		AudioSource m_audioSource;

		Coroutine[] m_activeCoroutines, m_pausedCoroutines;

		Coroutine m_delayCoroutine;

        //=============================================
        // Fields - Public
        //=============================================
        public MIDI midi { get { return m_midi; } }

		public bool playing{get{return m_playing;}}

		public bool finishedPlaying
		{
			get
			{
				return m_lastPlaying && !m_playing;
			}
		}

        //=============================================
        // Mono functions
        //=============================================
        void Awake()
        {
            m_audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            if (autoPlay)
            {
                PlayAfter(m_delayFor);
            }
        }

        void LateUpdate()
        {
            m_lastPlaying = m_playing;
        }

        //=============================================
        // Coroutines - Private
        //=============================================
		IEnumerator PlayTrack(MIDITrack track) 
		{
			MIDIMessage message;

			bool bBreak = false;
			float nextTime = 0, lastTime = 0, elapsed = 0;

			int iterator = 0;

			ReadOnlyCollection<MIDIMessage> trackMessages = track.messages;

			message = trackMessages [iterator];
			iterator++;

			while (!bBreak && !m_break) 
			{
				//Check if elapsed has ran over.
				if(elapsed >= nextTime)
				{
					if (iterator >= trackMessages.Count)
					{
						bBreak = true;
						yield break;
					}

                    Dispatch(message);

					message = trackMessages [iterator];
					iterator++;

					nextTime = m_midi.TDPS * message.wait - (elapsed - lastTime); // Accouting for timeloss due to framerate dependancy in order improve accuracy.
					lastTime = nextTime;
					elapsed = 0;
				}
				elapsed += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			m_break = false;
		}
        
        IEnumerator DelayThanPlay(float delayFor, MIDI newMidi = null, int track = -1)
        {
            yield return new WaitForSeconds(delayFor);
            if (track > -1)
                Play(track, newMidi);
            else
                Play(newMidi);
        }

        //=============================================
        // Methods - Public
        //=============================================
        public void Play(MIDI newMidi = null)
		{
            if (newMidi != null)
                m_midi = newMidi;
			if (m_midi != null && !m_playing) 
			{
				Stop();
                m_break = false;
                int tracks = IsPlayingFormat02() ? 1 : m_midi.GetNumberOfTracks();
				m_activeCoroutines = new Coroutine[tracks];
				for (int i = 0; m_midi.GetNumberOfTracksShort() > i; i++) 
				{
					m_activeCoroutines[i] = StartCoroutine (PlayTrack(m_midi.tracks [i]));
				}
				m_playing = true;

				if (m_audioSource != null && playAudio)
				{
					if(m_midi.audioClip != null)
					{
						m_audioSource.clip = m_midi.audioClip;
					}
					m_audioSource.Play();
				}
			}
		}

        public void Play(int track, MIDI newMidi = null)
		{
            if(newMidi != null)
                m_midi = newMidi;
			if (m_midi != null) 
			{
				Coroutine coroutine; 
                coroutine = StopExcept(track);
                if (coroutine == null && !m_playing)
				{
                    m_break = false;
					coroutine = StartCoroutine (PlayTrack(m_midi.tracks [track]));
					m_playing = true;
					if (m_audioSource != null && playAudio)
					{
						if(m_midi.audioClip != null)
						{
							m_audioSource.clip = m_midi.audioClip;
						}
						m_audioSource.Play();
					}
				}
				m_activeCoroutines = new Coroutine[]{coroutine};
			}
		}

		public void PlayAfter(float _delay, int track = -1, MIDI newMidi = null)
		{
			if (m_delayCoroutine != null)
				StopCoroutine (m_delayCoroutine);
			m_delayCoroutine = StartCoroutine (DelayThanPlay (_delay,newMidi,track));
		}

		public void SetMIDI(MIDI midi)
		{
            if(midi != null)
			    m_midi = midi;
		}

		public void Stop()
		{
			if(m_activeCoroutines != null)
			{
				for(int i = 0; i < m_activeCoroutines.Length; i++)
				{
					if(m_activeCoroutines[i] != null)
						StopCoroutine(m_activeCoroutines[i]);
				}
			}
			m_activeCoroutines = null;
			m_playing = false;
			m_break = true;
			if(m_audioSource != null)
				m_audioSource.Stop ();
		}

		public void Reset()
		{
			Stop ();
		}

        public void Pause(bool pause)
        { 

        }

        public void SwitchTrack(int track)
        {
            if (IsPlayingFormat00() || IsPlayingFormat01())
                return;
            Play(track);
        }

        public bool IsPlayingFormat00()
        {
            return midi != null && midi.header.format == 0;
        }

        public bool IsPlayingFormat01()
        {
            return midi != null && midi.header.format == 1;
        }

        public bool IsPlayingFormat02()
        {
            return midi != null && midi.header.format == 2;
        }

        //=============================================
        // Methods - Private
        //=============================================
		Coroutine StopExcept(int track)
		{
			if(m_activeCoroutines != null)
			{
				Coroutine keep = null;
				for(int i = 0; i < m_activeCoroutines.Length; i++)
				{
					if(i == track)
						keep = m_activeCoroutines[i];
					else
						StopCoroutine(m_activeCoroutines[i]);
				}
				return keep;
			}
			return null;
		}
	}
}
