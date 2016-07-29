using UnityEngine;
using UnityEditor;
using System.Collections;
using PrototypeFour;

[CustomEditor(typeof(TransformBlend))]
public class TransformBlendEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TransformBlend transformBlend = (TransformBlend)target;
        base.OnInspectorGUI();
        float oldValue = transformBlend.lerp, newValue = oldValue;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Value: ");
        newValue = EditorGUILayout.Slider(newValue,0,1);
        EditorGUILayout.EndHorizontal();

        transformBlend.SetBlend(newValue);
    }
}
