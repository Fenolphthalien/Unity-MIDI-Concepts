using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class EditorUtillity {

	public static void CreateScriptableObject<T>() where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T> ();
		ProjectWindowUtil.CreateAsset (asset, typeof(T).Name + ".asset");
		
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	public static void CreateUniqueScriptableObject<T>(string filePath) where T : ScriptableObject
	{
		T exists = (T)Resources.Load (filePath + typeof(T).Name);
		if (exists == null) {
			T asset = ScriptableObject.CreateInstance<T> ();
			ProjectWindowUtil.CreateAsset (asset, typeof(T).Name + ".asset");
			
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}
		else{
			Debug.LogError(typeof(T).Name + " Already exists");
		}
	}
}
