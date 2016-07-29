using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;

namespace UnityMIDI
{
    [System.Serializable]
    public class TrackIterator
    {
        int m_iteration;
        MIDITrack track;
        ReadOnlyCollection<MIDIMessage> trackMessages;

        MIDI m_parent;

        bool m_finished;
        
        public bool m_paused = false;
        public bool finished { get { return m_finished; } }

        public float elapsedTime { get { return m_elapsed; } }

        MIDIMessage message, LastMessage;

        float nextTime = 0, m_elapsed = 0;

        public void Play(MIDITrack _track = null, MIDI _parent = null)
        {
            if (_track != null)
            {
                track = _track;
                trackMessages = track.messages;
            }
            if (_parent != null)
            {
                m_parent = _parent;
            }
            Reset();
            message = trackMessages[m_iteration];
            m_iteration++;
        }

        public void Reset()
        {
            m_iteration = 0;
            nextTime = 0;
            m_elapsed = 0;
            m_finished = false;
        }

        public void Update(out bool dispatch, out MIDIMessage _message, float playbackRate)
        {
            if (m_paused)
            {
                dispatch = false;
                _message = LastMessage;
                return;
            }
            dispatch = false;
            LastMessage = message;
            _message = LastMessage;
            if (m_elapsed >= nextTime)
            {
                m_finished = m_iteration >= trackMessages.Count;
                if (m_finished)
                {
                    return;
                }

                _message = message;
                dispatch = true;

                message = trackMessages[m_iteration];
                m_iteration++;

                nextTime = (m_parent.TDPS * message.wait) - (m_elapsed - nextTime); // Accouting for timeloss due to framerate dependancy in order improve accuracy.
                //lastTime = nextTime;
                m_elapsed = 0;
                if (message.midiEvent == MIDIEvent.META_TRACK_FINISHED)
                    m_finished = true;
            }

            m_elapsed += Time.deltaTime * playbackRate;
        }
    }
}
