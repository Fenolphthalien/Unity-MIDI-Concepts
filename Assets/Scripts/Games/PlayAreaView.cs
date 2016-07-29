using UnityEngine;
using System.Collections;

namespace NOsu
{
	[ExecuteInEditMode,RequireComponent(typeof(MeshRenderer),typeof(MeshFilter))]
	public class PlayAreaView : MonoBehaviour
	{
		[SerializeField]
		protected Material m_outlineMat;

		Mesh m_outline;

		bool m_dirty;

		const int kSides = 64;
		const float kDivider = 360f / (kSides);
		float[] m_sine, m_cosine;

		public float radius = 1, border = 0.1f;
		public float borderRadius{get{return radius + border;}}

		float m_radius, m_border;

		void Awake()
		{
			m_sine = new float[kSides];
			m_cosine = new float[kSides];
			for(int i = 0; i < kSides; i++)
			{
				m_sine[i] = Mathf.Sin((kDivider * i)  * Mathf.Deg2Rad);
				m_cosine[i] = Mathf.Cos((kDivider * i)  * Mathf.Deg2Rad);
			}
		}

		void LateUpdate ()
		{
			m_dirty = m_radius != radius || m_border != border;
			if (m_dirty) {
				BuildOutline ();
			}

			if (m_outline != null && m_outlineMat != null) {
				Matrix4x4 m_matrix = Matrix4x4.identity;
				m_matrix.SetTRS (this.transform.position, this.transform.rotation, Vector3.one);
				Graphics.DrawMesh (m_outline, m_matrix, m_outlineMat,0);
			}
			m_radius = radius;
			m_border = border;
		}

		void BuildOutline()
		{
			transform.localScale = Vector3.one * radius;

			if (m_outline == null)
				m_outline = new Mesh ();
			MeshGeneration.CircularOutline (m_outline,kSides,radius,borderRadius,Color.white,m_cosine, m_sine, HideFlags.HideAndDontSave);

//			if(m_outline == null)
//				m_outline = new Mesh ();
//			m_outline.hideFlags = HideFlags.HideAndDontSave;
//
//			int x = 0, i = 0, tIt = 0, tIn = 0;
//			Vector3[] verts = new Vector3[kSides*4 + 4];
//			Vector2[] uvs = new Vector2[kSides * 4 + 4];
//			int[] tris = new int[kSides * 6];
//
//			while(x < kSides)
//			{
//				int vertIndex0 = x%kSides, vertIndex1 = (x+1)%kSides; // Prevent sine/cosine buffer overflow.
//
//				verts[i] = new Vector3(m_cosine[vertIndex0] * radius,0,m_sine[vertIndex0]*radius);
//				verts[i+1] = new Vector3(m_cosine[vertIndex1] * radius,0,m_sine[vertIndex1]*radius);
//				verts[i+2] = new Vector3(m_cosine[vertIndex0]*borderRadius,0,m_sine[vertIndex0]*borderRadius);
//				verts[i+3] = new Vector3(m_cosine[vertIndex1]*borderRadius,0,m_sine[vertIndex1]*borderRadius);
//				x++;
//
//				tris[tIn] = tIt;
//				tris[tIn + 1] = tIt + 1;
//				tris[tIn + 2] = tIt + 2;
//				tris[tIn + 3] = tIt + 1;
//				tris[tIn + 4] = tIt + 3;
//				tris[tIn + 5] = tIt + 2;
//
//				tIt += 4;
//				tIn += 6;
//
//				i += 4;
//			}
//			
//			m_outline.vertices = verts;
//			m_outline.uv = uvs;
//			m_outline.triangles = tris;
//			m_outline.RecalculateNormals ();
//			m_outline.RecalculateBounds ();
		}
	}
}
