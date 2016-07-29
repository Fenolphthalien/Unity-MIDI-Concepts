using UnityEngine;
using UnityMIDI;
using System.Collections;

public static class MIDIColourMaps
{
    static int octaveMapDimensions = 4;

    public static void SetPixel_OctaveMask(Texture2D tex, Tone tone, bool value, bool applyTexture = false)
    {
        if (tex == null)
            return;
        int x, y;
        Color colour = value ? Color.white : Color.black;
        OctaveMapCoords(tone, out x, out y);
        tex.SetPixel(x,y,colour);
        if(applyTexture)
            tex.Apply();
    }

    public static void SetPixels_OctaveMask(Texture2D tex, bool value, params Tone[] tones)
    {
        if (tex == null)
            return;

        foreach (Tone t in tones)
        {
            SetPixel_OctaveMask(tex, t, value);
        }
        tex.Apply();
    }

    public static void SetPixel_MIDIMask(Texture2D tex, Tone tone, int octave, bool value, bool applyTexture = false)
    {
        if (tex == null)
            return;
        int x, y;
        Color colour = value ? Color.white : Color.black;
        
        MIDIMapCoords(tone, octave, out x, out y);
        tex.SetPixel(x, y, colour);
        if (applyTexture)
            tex.Apply();
    }

    public static void SetPixels_MIDIMask(Texture2D tex, bool value, params MIDIMessage[] messages)
    {
        if (tex == null)
            return;

        int octave;
        Tone tone;
        foreach (MIDIMessage message in messages)
        {
            tone = message.GetNote();
            octave = message.GetOctave();
            SetPixel_MIDIMask(tex, tone,octave, value);
        }
        tex.Apply();
    }

    static void OctaveMapCoords(Tone t, out int x, out int y)
    {
        y = (int)t / octaveMapDimensions;
        x = (int)t % octaveMapDimensions;
    }

    static void MIDIMapCoords(Tone t, int octave, out int x, out int y)
    {
        y = octave;
        x = (int)t;
    }
}
