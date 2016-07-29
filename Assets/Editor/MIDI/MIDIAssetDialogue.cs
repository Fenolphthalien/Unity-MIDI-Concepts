using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityMIDI;

public class MIDIAssetDialogue
{
	[MenuItem("Assets/Create/MIDI/Blank MIDI")]
	static void CreateNewMIDIAsset()
	{
		MIDIAssetUtility.CreateMIDIAsset ();
	}

	[MenuItem("Assets/Create/MIDI/Instrument")]
	static void CreateNewInstrument()
	{
		EditorUtillity.CreateScriptableObject<Instrument> ();
	}

    [MenuItem("GameObject/Create Other/MIDI/MIDI Player 02")]
    static void CreateNewMIDIPlayer02()
    {
        GameObject gameObject = new GameObject("MIDI Player 02");
        gameObject.AddComponent<MIDIPlayer02>();
        gameObject.AddComponent<AudioSource>();
    }

    [MenuItem("GameObject/Create Other/MIDI/MIDI Input Stream")]
    static void CreateNewMIDIInputStream()
    {
        GameObject gameObject = new GameObject("MIDI Input Stream");
        gameObject.AddComponent<MIDIInStreamControl>();
    }
}
