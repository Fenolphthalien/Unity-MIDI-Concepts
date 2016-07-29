using UnityEngine;
using UnityEditor;
using System.Collections;

public class ScrollingMaskedTextureEditor : MaterialEditor
{
	private Vector2 lastScroll;

	public override void OnInspectorGUI ()
	{
		Material material = (Material)target;
		if (!isVisible)
			return;

		if(GUILayout.Button("Swap Colours"))
		{
			SwapColours(material);
		}
		base.OnInspectorGUI ();

		Vector4 scroll = material.GetVector("_Scroll");
		Vector2 thisScroll = new Vector2 (scroll.x, scroll.y);
		if(thisScroll != lastScroll)
		{
			lastScroll = thisScroll.normalized;
			material.SetVector("_Scroll",new Vector4(lastScroll.x,lastScroll.y,scroll.z,scroll.w));
		}
	}

	void SwapColours(Material mat)
	{
		Color color0 = mat.GetColor("_Color"), color1 = mat.GetColor("_Color1");
		mat.SetColor ("_Color", color1);
		mat.SetColor ("_Color1", color0);
	}
}
