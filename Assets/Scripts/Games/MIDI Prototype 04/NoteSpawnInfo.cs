using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeFour
{
    public class NoteSpawnInfo
    {
        protected readonly float barPosition;
        protected int[] m_notes;
        public int firstNoteIndex { get { return m_notes != null ? m_notes[0] : 0; } }
        public int count { get { return m_notes != null ? m_notes.Length : -1; } }
        float m_length;
        public float length { get { return m_length; } } //Length is relative to beats per bar. quarter note = 0.25; half note = 0.5 in 4/4 ect

        NoteSpawnInfo m_last, m_next;

        public NoteSpawnInfo last { get { return m_last; } }
        public NoteSpawnInfo next { get { return m_next; } }

        public int this[int key]
        {
            get
            {
                if (m_notes == null)
                    return 0;
                return m_notes[key];
            }
        }

        int highest, lowest;

        public NoteSpawnInfo(float _barPosition, int _index, float _length)
        {
            barPosition = _barPosition;
            m_length = _length;
            m_notes = new int[1] { _index };
            lowest = _index;
            highest = _index;
        }

        ~NoteSpawnInfo()
        {
            m_next = null;
            m_last = null;
            m_notes = null;
        }

        public void GetBarPosition(out int bar, out float position)
        {
            int b = Mathf.FloorToInt(barPosition);
            position = barPosition - b;
            bar = b;
        }

        //<summary>
        // returns the position in world space of the lowest note index.
        //</summary>
        public void GetBarPosition(out Vector3 position, float width, float height, float heightDividerScalar = 1)
        {
            int bar = 0;
            float x = 0, y = 0;
            GetBarPosition(out bar, out x);
            x *= width;
            y = lowest * height;
            position = new Vector3(bar * width + x, y);
        }

        //<summary>
        // returns the position in world space of the overloaded note index.
        //</summary>
        public void GetBarPosition(out Vector3 position, float width, float height, int index, float heightDividerScalar = 1)
        {
            int bar = 0;
            float x = 0, y = 0;
            GetBarPosition(out bar, out x);
            x *= width;
            y = m_notes[index] * height;
            position = new Vector3(bar * width + x, y);
        }

        public void AddNoteToRange(int _note)
        { 
            int[] buffer = new int[m_notes.Length + 1];
            int iterator = 0;
            while(iterator < m_notes.Length)
            {
                buffer[iterator] = m_notes[iterator];
                iterator++;
            }
            buffer[iterator] = _note;
            m_notes = buffer;
            if (_note > highest)
                highest = _note;
            if (_note < lowest)
                lowest = _note;
        }

        public void AddNoteIndex(int index, float _length)
        {
            int[] buffer = new int[m_notes.Length + 1];
            int iterator = 0;
            while (iterator < m_notes.Length)
            {
                buffer[iterator] = m_notes[iterator];
                iterator++;
            }
            buffer[iterator] = index;
            m_notes = buffer;
            m_length = m_length < _length ? _length : m_length;
        }

        public void ShiftNoteIndices(int by)
        {
            for (int i = 0; i < m_notes.Length; i++) 
            { 
                m_notes[i] += by;
            }
            highest += by;
            lowest += by;
        }

        public bool isPolyphonic()
        {
            return count > 1;
        }

        public int GetLowest()
        {
            return lowest;
        }

        public int GetHighest()
        {
            return highest;
        }

        public int GetRange()
        {
            return highest - lowest;
        }

        public bool RangeBetweenLastAndNext(out int range)
        {
            range = 0;
            if (m_last == null || m_next == null)
                return false;
            
            int A = m_last.isPolyphonic() ? m_last.GetMeanNoteIndex() : m_last.firstNoteIndex, 
                B = m_next.isPolyphonic() ? m_next.GetMeanNoteIndex() : m_next.firstNoteIndex;
            range = B - A;
            return true;
        }

        public int RangeBetweenThisAndLast()
        {
            if (m_last == null)
                return 0;
            int B = m_last.isPolyphonic() ? m_last.GetMeanNoteIndex() : m_last.firstNoteIndex, A = isPolyphonic() ? GetMeanNoteIndex() : firstNoteIndex;
            return B - A;
        }

        public int RangeBetweenThisAndNext()
        {
            if (m_next == null)
                return 0;
            int B = m_next.isPolyphonic() ? m_next.GetMeanNoteIndex() : m_next.firstNoteIndex, A = isPolyphonic() ? GetMeanNoteIndex() : firstNoteIndex;
            return B - A;
        }

        public int GetMeanNoteIndex()
        {
            int middleIndex = m_notes.Length / 2;
            return m_notes[middleIndex];
        }

        public void SetAdjecentReferences(NoteSpawnInfo last, NoteSpawnInfo next)
        {
            m_last = last;
            m_next = next;
        }

        bool CompareHighestNote()
        {
            return false;
        }
    }
}
