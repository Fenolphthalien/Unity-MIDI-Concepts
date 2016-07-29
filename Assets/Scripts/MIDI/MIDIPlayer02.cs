using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace UnityMIDI
{
    [System.Serializable]
    public class MIDIPlayerEvent : UnityEvent {}

    public class MIDIPlayer02 : MIDIDispatcher
    {
        private enum EPlayerState { STOPPED, DELAYINGPLAY, PLAYING, PAUSED };

        //=============================================
        // Members - Public
        //=============================================

        public bool autoPlay = true, playAudio = true, loop = false;

        public MIDIPlayerEvent OnPlaybackStart, OnPlaybackStop, OnPlaybackFinish, OnPlaybackLoop;

        public PlayBackSpeed playbackSpeed
        {
            get
            {
                return m_playbackSpeed;
            }
            set
            {
                SetPlaybackSpeed(value);
            }
        }

        //=============================================
        // Members - Private
        //=============================================
        [SerializeField]
        MIDI m_midi;

        [SerializeField]
        float m_delayFor = 2;
        float m_delayTime;

        [SerializeField]
        int playTrack = -1;

        PlayBackSpeed m_playbackSpeed = PlayBackSpeed.NORMAL;
        float m_playbackSpeedFloat = PlayBackSpeed.NORMAL.ToFloat();

        EPlayerState m_state, m_lastState;

        TrackIterator[] iterators = null;

        AudioSource m_audioSource;

        //=============================================
        // Fields - Public
        //=============================================
        public MIDI midi { get { return m_midi; } }

        public bool playing { get { return m_state == EPlayerState.PLAYING; } }

        public bool finishedPlaying{get{return m_lastState == EPlayerState.PLAYING && m_state == EPlayerState.STOPPED;}}

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
                PlayAfter(m_delayFor, playTrack);
            }
        }

        void Update()
        {
            if (m_state == EPlayerState.DELAYINGPLAY)
            {
                HandleDelayState();
            }
            else if (m_state == EPlayerState.PLAYING)
            {
                HandlePlayState();
            }
        }

        void LateUpdate()
        {
            m_lastState = m_state;
        }

        void HandleDelayState()
        {
            if (m_delayTime <= 0)
                Play();
            else
                m_delayTime -= Time.deltaTime;
        }

        void HandlePlayState()
        {
            if (iterators == null || iterators.Length == 0)
            {
                m_state = EPlayerState.STOPPED;
                return;
            }
            int i = 0;
            bool dispatch = false;
            MIDIMessage message;
            
            //This causes null exception sometimes, don't know why.
            while(iterators != null && i < iterators.Length)
            {
                if (!iterators[i].finished)
                {
                    iterators[i].Update(out dispatch, out message, m_playbackSpeedFloat);
                    if (dispatch)
                        Dispatch(message);
                }
                i++;
            }
            if (IteratorsFinished())
            {
                Debug.Log("Track Finished");
                if (loop)
                {
                    if (OnPlaybackLoop != null)
                        OnPlaybackLoop.Invoke();
                    Reset();
                    return;
                }
                if (OnPlaybackFinish != null)
                    OnPlaybackFinish.Invoke();
                Stop();
            }
        }

        bool IteratorsFinished()
        {
            if(iterators == null)
                return true;
            bool finished = true;
            for (int i = 0; i < iterators.Length; i++)
            {
                finished = iterators[i].finished ? finished : false;
            }
            return finished;
        }

        //=============================================
        // Methods - Public
        //=============================================

        public void Play()
        {
            Play(null);
        }

        public void Play(MIDI newMidi)
        {
            if (newMidi != null)
                m_midi = newMidi;
            if (m_midi != null && m_state != EPlayerState.PLAYING)
            {
                Stop();
                int tracks = IsPlayingFormat02() ? 1 : m_midi.GetNumberOfTracks();
                iterators = new TrackIterator[tracks];
                for(int i = 0; i < tracks; i++)
                {
                    iterators[i] = new TrackIterator();
                    iterators[i].Play(midi.GetTrack(i),midi);
                }

                m_state = EPlayerState.PLAYING;

                if (m_audioSource != null && playAudio)
                {
                    if (m_midi.audioClip != null)
                    {
                        m_audioSource.clip = m_midi.audioClip;
                        m_audioSource.loop = loop;
                    }
                    m_audioSource.Play();
                    if (OnPlaybackStart != null)
                        OnPlaybackStart.Invoke();
                }
            }
        }

        public void Play(int track, MIDI newMidi = null)
        {
            if (newMidi != null)
                m_midi = newMidi;
            if (m_midi != null)
            {
                if (m_state != EPlayerState.PLAYING)
                {
                    iterators = new TrackIterator[1];
                    iterators[0] = new TrackIterator();
                    iterators[0].Play(midi.GetTrack(track), midi);
                   
                    m_state = EPlayerState.PLAYING;
                    if (playAudio)
                    {
                        if (m_midi.audioClip != null)
                        {
                            m_audioSource.clip = m_midi.audioClip;
                        }
                        PlayAudio();
                    }
                    if (OnPlaybackStart != null)
                        OnPlaybackStart.Invoke();
                }
            }
        }

        public void PlayAfter(float _delay, int track = -1, MIDI newMidi = null)
        {
            m_delayTime = _delay;
            m_state = EPlayerState.DELAYINGPLAY;
        }

        public void SetMIDI(MIDI midi)
        {
            if (midi != null)
                m_midi = midi;
        }

        public void Stop()
        {
            m_state = EPlayerState.STOPPED;
            m_delayTime = 0;
            if (iterators == null)
                return;
            for (int i = 0; i < iterators.Length; i++)
                iterators[i] = null;
            iterators = null;
            if (OnPlaybackStop != null)
                OnPlaybackStop.Invoke();
            StopAudio(false);
        }

        public void StopAndReset()
        {
            Stop();
        }

        public void Reset()
        {
            if (iterators == null)
                return;
            for (int i = 0; iterators.Length > i; i++)
                iterators[i].Reset();
            RestartAudio();
        }

        public void Pause(bool pause)
        {
            m_state = pause ? EPlayerState.PAUSED : EPlayerState.PLAYING;
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
        void SetPlaybackSpeed(PlayBackSpeed newSpeed)
        {
            if (newSpeed == m_playbackSpeed)
                return;
            m_playbackSpeed = newSpeed;
            m_playbackSpeedFloat = m_playbackSpeed.ToFloat();
        }

        void PlayAudio()
        {
            if (m_audioSource == null || m_audioSource.isPlaying)
                return;
            m_audioSource.Play();
        }

        void StopAudio(bool stopAsPaused)
        {
            if (m_audioSource == null)
                return;
            if (stopAsPaused)
                m_audioSource.Pause();
            else
                m_audioSource.Stop();
        }

        void RestartAudio()
        {
            if (m_audioSource == null)
                return;
            m_audioSource.Stop();
            m_audioSource.Play();
        }
    }
}