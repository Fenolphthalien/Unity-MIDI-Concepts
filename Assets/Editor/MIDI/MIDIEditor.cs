using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityMIDI;
using UnityMIDI.Import;

[CustomEditor(typeof(MIDI))]
public class MIDIEditor : Editor
{

    bool displayTrackData = false;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI ();

        MIDI midi = (MIDI)target;

        midi.sourcePath = EditorGUILayout.TextField("Source Path", midi.sourcePath);

        EditorGUILayout.BeginHorizontal();
        midi.usingRelativePath = EditorGUILayout.Toggle("Load from relative folder", midi.usingRelativePath);
        EditorUtility.SetDirty(midi);

        if (GUILayout.Button("Load From Path"))
        {
            MIDIImporter.LoadIntoMIDIAsset(midi);
            EditorUtility.SetDirty(midi);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Audio clip: ");
        midi.audioClip = (AudioClip)EditorGUILayout.ObjectField(midi.audioClip, typeof(AudioClip), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        if (!midi.initialised)
        {
            EditorGUILayout.LabelField("Warning: This file contains no tracks.");
            return;
        }
        displayTrackData = EditorGUILayout.BeginToggleGroup("Tracks", displayTrackData);
        if (displayTrackData)
        {
            int startTrack = midi.GetFormat() == 0 ? 0 : 1;
            for (int i = startTrack; midi.tracks.Count > i; i++)
            {
                MIDIEditorGUILayout.MIDITrackField(midi.tracks[i], i);
                EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Separator();

        long oldBPM = (long)midi.BPM, newBPM;
        //EditorGUILayout.LabelField("BPM: ", midi.BPM.ToString());
        newBPM = EditorGUILayout.LongField("BPM", (long)midi.BPM);
        if (oldBPM != newBPM)
        {
            midi.SetTempo((ulong)newBPM, ESetTempo.BPM);
        }

        midi.displayName = EditorGUILayout.TextField("Display name", midi.displayName);
        midi.timeSignature = MIDIEditorGUILayout.TimeSignatureField(midi.timeSignature);
        EditorGUILayout.LabelField("Copyright: ", midi.copyright.ToString());
        EditorGUILayout.LabelField("Number Of Tracks: ", midi.tracks.Count.ToString());
        EditorGUILayout.LabelField("Length: ", string.Format("{0} seconds.", midi.seconds));
        EditorGUILayout.LabelField("PPQN: ", midi.header.ppqn.ToString());

        //EditorGUILayout.BeginHorizontal();
        //assignTrack = EditorGUILayout.IntField(assignTrack);
        //int minTracks = midi.GetFormat() == 1 ? 1 : 0;
        //assignTrack = (assignTrack > minTracks ? assignTrack : minTracks) < midi.trackCount ? assignTrack : midi.trackCount - 1;

        //ta = (TextAsset)EditorGUILayout.ObjectField(ta, typeof(TextAsset), false);
        //if (GUILayout.Button("Assign"))
        //{
        //    if (ta != null)
        //    {
        //        MIDIHelpers.SetTrackVelocitiesToCharacterCount(midi.GetTrack(assignTrack), ta);
        //        AssetDatabase.SaveAssets();
        //        AssetDatabase.Refresh();
        //    }
        //}
        //EditorGUILayout.EndHorizontal();
    }
}

public static class MIDIEditorGUILayout
{

    public static void MIDITrackField(MIDITrack track, int i)
    {

        EditorGUILayout.LabelField(string.Format("Track {0}", i));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Track name: ");
        track.name = EditorGUILayout.TextField(track.name);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField(string.Format("Track Length: {0} seconds.", track.seconds));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Instrument: ");
        track.instrument = (Instrument)EditorGUILayout.ObjectField(track.instrument, typeof(Instrument), false);
        EditorGUILayout.EndHorizontal();
    }

    public static TimeSignature TimeSignatureField(TimeSignature timeSignature)
    {
        int bpb = timeSignature.beatsPerBar;
        Metre metre = timeSignature.metre;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Time Signature: ", new GUILayoutOption[] { GUILayout.Width(256f) });
        bpb = EditorGUILayout.IntField(bpb, new GUILayoutOption[] { GUILayout.Width(16f) });
        EditorGUILayout.LabelField("/", new GUILayoutOption[] { GUILayout.Width(8f) });
        metre = (Metre)EditorGUILayout.EnumPopup(metre, new GUILayoutOption[] { GUILayout.Width(58f) });
        EditorGUILayout.EndHorizontal();

        bool dirty = (bpb != timeSignature.beatsPerBar) || (metre != timeSignature.metre);
        if (!dirty)
            return timeSignature;
        return new TimeSignature(bpb, metre);
    }
}
