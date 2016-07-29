using UnityEngine;
using System.Collections;

namespace NOsu{
	public sealed class NosuPlayerRenderer : MonoBehaviour
	{
		[SerializeField]
        Material m_material = null;
		Mesh m_Mesh;

		const int pixelPerUnit = 100;

		[SerializeField]
		Vector4 fillSpriteBounds = Vector4.zero;
		[SerializeField]
		Vector2 shellSpriteOffset = Vector2.zero, textureSize = Vector2.zero;

		int m_targetHealth = NosuPlayer.kMaxHealth, m_displayHealth = NosuPlayer.kMaxHealth;

		float m_uvX0, m_uvY0, m_uvX1, m_uvY1,m_shell_uvX0, m_shell_uvY0, m_shell_uvX1, m_shell_uvY1;
		float m_xBound, m_yBound;
		
		void Start()
		{
			m_uvX0 = fillSpriteBounds.x / textureSize.x;
			m_uvY0 = fillSpriteBounds.y / textureSize.y;
			m_uvX1 = (fillSpriteBounds.x + fillSpriteBounds.z) / textureSize.x;
			m_uvY1 = (fillSpriteBounds.y + fillSpriteBounds.w) / textureSize.y;

			m_shell_uvX0 = (fillSpriteBounds.x + shellSpriteOffset.x) / textureSize.x;
			m_shell_uvY0 = (fillSpriteBounds.x + shellSpriteOffset.y) / textureSize.y;
			m_shell_uvX1 = (fillSpriteBounds.x + shellSpriteOffset.x + fillSpriteBounds.z) / textureSize.x;
			m_shell_uvY1 = (fillSpriteBounds.x + shellSpriteOffset.y + fillSpriteBounds.z) / textureSize.y;

			m_xBound = fillSpriteBounds.z / pixelPerUnit;
			m_yBound = fillSpriteBounds.w / pixelPerUnit;

			BuildMesh ();
		}

		void LateUpdate ()
		{
			if(m_targetHealth != m_displayHealth)
			{
				BuildMesh();
			}
			if(m_Mesh != null && m_material != null){
				Matrix4x4 matrix = Matrix4x4.identity;
				matrix.SetTRS(this.transform.position,this.transform.rotation,Vector3.one);
				Graphics.DrawMesh(m_Mesh,matrix,m_material,0);
			}
		}

		public void BuildMesh()
		{
			if (m_Mesh == null) {
				m_Mesh = new Mesh ();
				m_Mesh.hideFlags = HideFlags.HideAndDontSave;
			}
				
			Vector3[] verts;
			Vector2[] uvs;
			int[] tris;

			float fillPercentage = 1;
			int cuts = 0; 

			Color color = Color.white;

			if(m_targetHealth != m_displayHealth)
			{
				cuts++;
				color = m_targetHealth < m_displayHealth ? Color.red : Color.green;
				fillPercentage = (float)m_targetHealth / NosuPlayer.kMaxHealth;
			}
			verts = new Vector3[4 * (cuts+1)];
			uvs = new Vector2[4  * (cuts+1)];
			tris = new int[6 * (cuts+1)];

			float yUVDiff = m_shell_uvY1 - m_shell_uvY0;
			float yFill = -m_yBound * 0.5f + (m_yBound * fillPercentage), yUVFill = m_shell_uvY0 + yUVDiff * fillPercentage;

			//Draw fill
			verts [0] = new Vector3 (-m_xBound * 0.5f, 0,-m_yBound * 0.5f);
			verts [1] = new Vector3 (-m_xBound * 0.5f,0, yFill);
			verts [2] = new Vector3 (m_xBound * 0.5f, 0,-m_yBound * 0.5f);
			verts [3] = new Vector3 (m_xBound * 0.5f,0, yFill);

			//Draw Shell
			if (cuts > 0) 
			{
				verts [4] = new Vector3 (-m_xBound * 0.5f, 0,yFill);
				verts [5] = new Vector3 (-m_xBound * 0.5f,0 ,m_yBound * 0.5f);
				verts [6] = new Vector3 (m_xBound * 0.5f,0, yFill);
				verts [7] = new Vector3 (m_xBound * 0.5f, 0,m_yBound * 0.5f);
			}

			uvs [0] = new Vector2(m_uvX0,m_uvY0);
			uvs [1] = new Vector2(m_uvX0,yUVFill);
			uvs [2] = new Vector2(m_uvX1,m_uvY0);
			uvs [3] = new Vector2(m_uvX1,yUVFill);
			if(cuts > 0)
			{
				uvs [4] = new Vector2(m_shell_uvX0,yUVFill);
				uvs [5] = new Vector2(m_shell_uvX0,m_shell_uvY1);
				uvs [6] = new Vector2(m_shell_uvX1,yUVFill);
				uvs [7] = new Vector2(m_shell_uvX1,m_shell_uvY1);
			}
			tris [0] = 0;
			tris [1] = 1;
			tris [2] = 2;
			tris [3] = 1;
			tris [4] = 3;
			tris [5] = 2;
			if(cuts > 0){
				tris [6] = 4;
				tris [7] = 5;
				tris [8] = 6;
				tris [9] = 5;
				tris [10] = 7;
				tris [11] = 6;
			}



			m_Mesh.vertices = verts;
			m_Mesh.uv = uvs;
			m_Mesh.triangles = tris;

			m_Mesh.RecalculateNormals ();
			m_Mesh.RecalculateBounds ();

			m_displayHealth = m_targetHealth;
		}

		public void SetHealth(int health)
		{
			m_targetHealth = health >= 0 ? health : 0;
		}
	}
}
