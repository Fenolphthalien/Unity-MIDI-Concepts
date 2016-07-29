using UnityEngine;
using System.Collections;
using UnityMIDI;

namespace PrototypeFour
{
    namespace Debugging
    {
        public class MIDIProcessorDebug : MonoBehaviour
        {
            public MIDI midi;
            public int track = 0;

            void Start()
            {
                if (midi == null)
                    return;
                
                int playTrack = Mathf.Max(track % midi.GetNumberOfTracks(),0);
                playTrack = midi.GetFormat() == 1 && playTrack < 1 ? playTrack + 1 : playTrack; 
                
                IMIDIProcessor midiProcessor = new PreGame.MIDIProcessor();
                midiProcessor.ProcessMIDI(midi, playTrack);
                
                midiProcessor = null;
            }
        }
    }
}
