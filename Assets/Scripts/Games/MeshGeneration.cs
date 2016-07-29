using UnityEngine;
using System.Collections;

public static class MeshGeneration 
{
	public static bool CircularOutline(Mesh mesh, int sides, float radius, float borderRadius, Color vertexColour,float[] cosine, float[] sine, HideFlags hideFlag)
	{
		if (sides != cosine.Length && sides != sine.Length)
		{
			Debug.LogError("Number of sides does not match the number of indexes in the Consine or Sine Array");
			return false;
		}
		else if(cosine.Length != sine.Length)
		{
			Debug.LogError("Cosine and Sine array or not the same Length");
			return false;
		}
		else if(mesh == null)
		{
			Debug.LogError("Mesh is null");
			return false;
		}

		mesh.hideFlags = hideFlag;
		int x = 0, i = 0, tIt = 0, tIn = 0;
		Vector3[] verts = new Vector3[sides*4 + 4];
		Vector2[] uvs = new Vector2[sides * 4 + 4];
		Color[] colours = new Color[sides * 4 + 4];
		int[] tris = new int[sides * 6];

		for(int popIndex = 0; popIndex < colours.Length; popIndex++)
		{
			colours[popIndex] = vertexColour;
		}

		while(x < sides)
		{
			int vertIndex0 = x%sides, vertIndex1 = (x+1)%sides; // Prevent sine/cosine buffer overflow.
			
			verts[i] = new Vector3(cosine[vertIndex0] * radius,0,sine[vertIndex0]*radius);
			verts[i+1] = new Vector3(cosine[vertIndex1] * radius,0,sine[vertIndex1]*radius);
			verts[i+2] = new Vector3(cosine[vertIndex0]*borderRadius,0,sine[vertIndex0]*borderRadius);
			verts[i+3] = new Vector3(cosine[vertIndex1]*borderRadius,0,sine[vertIndex1]*borderRadius);
			x++;
			
			tris[tIn] = tIt;
			tris[tIn + 1] = tIt + 1;
			tris[tIn + 2] = tIt + 2;
			tris[tIn + 3] = tIt + 1;
			tris[tIn + 4] = tIt + 3;
			tris[tIn + 5] = tIt + 2;
			
			tIt += 4;
			tIn += 6;
			
			i += 4;
		}
		
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.colors = colours;
		mesh.triangles = tris;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		return true;
	}
}
