using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityMIDI;
using UnityMIDI.Import;

[CustomEditor(typeof(MIDIDispatcher))]
public class MIDIDispatcherEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.LabelField("Subscribers.");
    }
}
