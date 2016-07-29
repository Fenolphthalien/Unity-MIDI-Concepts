using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeFour
{
    public class ProcessedMIDI
    {
        public readonly float seconds, spqn;

        public readonly ulong tempo;

        public readonly uint bars, noteRange;

        public readonly NoteSpawnInfo[] spawnInfo;

        public readonly TimeSignature timeSignature;

        public ProcessedMIDI(string _name, float _seconds, ulong _tempo, uint _bars,uint _noterange,TimeSignature _timeSignature, NoteSpawnInfo[] _spawnInfo)
        {
            seconds = _seconds;
            tempo = _tempo;
            bars = _bars;
            noteRange = _noterange;
            spawnInfo = _spawnInfo;
            timeSignature = _timeSignature;
        }

        public void NormaliseSpawnData()
        { 
            int lowestIndex = 0;
            for(int i = 0; i < spawnInfo.Length; i++)
            {
                lowestIndex = lowestIndex > spawnInfo[i].GetLowest() ? spawnInfo[i].GetLowest() : lowestIndex;
            }
            for(int i = 0; i < spawnInfo.Length; i++)
            {
                spawnInfo[i].ShiftNoteIndices(-lowestIndex);
            }
        }
    }
}
