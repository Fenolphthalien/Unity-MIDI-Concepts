using UnityEngine;
using UnityMIDI;
using System.Collections;

public static class DrumExtensions
{
    public static bool IsKick(this MIDIMessage message)
    {
        return message.GetNote() == Tone.C && message.GetOctave() == 1;
    }

    public static bool IsSnare(this MIDIMessage message)
    {
        return message.GetNote() == Tone.D && message.GetOctave() == 1;
    }
}
