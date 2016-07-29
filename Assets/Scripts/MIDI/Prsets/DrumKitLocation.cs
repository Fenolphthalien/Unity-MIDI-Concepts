using UnityEngine;
using System.Collections;
using UnityMIDI;

public static class DrumMasks  
{
    public static void Kick(out Tone note, out int octave) 
    {
        note = Tone.C;
        octave = 1;
    }

    public static void Snare(out Tone note, out int octave)
    {
        note = Tone.D;
        octave = 1;
    }
}
