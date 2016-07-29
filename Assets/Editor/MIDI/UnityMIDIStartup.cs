using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class UnityMIDIEditorStartup
{
    static UnityMIDIEditorStartup()
    {
        UnityMIDIPreferencesEditor.SetShaderVariables();
    }
}
