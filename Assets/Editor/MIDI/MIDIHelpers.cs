using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityMIDI;

public static class MIDIHelpers
{
    public static void SetTrackVelocitiesToCharacterCount(MIDITrack track, TextAsset asset)
    {
        int words = 0, i = 0, wi = 0;
        List<int> characterCounts = new List<int>();
        TextAssetReader assetReader = new TextAssetReader(asset);
        assetReader.NumberOfWordsInTextWithSpaces(ref characterCounts, ref words);

        if (characterCounts == null || characterCounts.Count == 0)
        {
            Debug.Log("Returning");
            return;
        }

        while (i < track.messages.Count)
        {
            if (track.messages[i].IsNoteOn() && track.messages[i].GetNote() == Tone.C)
            {
                track.messages[i].SetVelocity(characterCounts[wi]);
                wi++;
                wi = wi < characterCounts.Count ? wi : characterCounts.Count - 1;
            }
            i++;
        }
        Debug.Log("Finished");
    }
}
