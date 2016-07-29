using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityMIDI;

/* http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset */
/* http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset2 */

/* Changed the generic type to MIDI and combined the above script togther to best fit my needs */

public static class MIDIAssetUtility
{

	//----------------------------------------------------------------
	//	Create Asset
	//----------------------------------------------------------------

	public static void CreateMIDIAsset()
	{
		MIDI asset = ScriptableObject.CreateInstance<MIDI> ();

		ProjectWindowUtil.CreateAsset(asset, "New " + typeof(MIDI).Name + ".asset");

		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}
}
