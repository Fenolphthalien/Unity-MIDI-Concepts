using UnityEngine;
using UnityEditor;
using System.Collections;

public class Background01ShaderEditor : MaterialEditor
{
    private bool m_displaySnare, m_displayKick;

    public override void OnInspectorGUI()
    {
        Material material = (Material)target;
        if (!isVisible)
            return;
        base.OnInspectorGUI();
       
        EditorGUILayout.BeginHorizontal();
        m_displayKick = EditorGUILayout.Toggle("Display Kick",m_displayKick);
        EditorGUILayout.EndHorizontal();
    }

}
