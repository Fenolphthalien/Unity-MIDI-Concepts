using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace NOsu
{
	public class NosuEmmiterRenderer : MonoBehaviour
	{
		
		const int kLifeTime = 1;
		float lifeElapsed = kLifeTime;

		Color m_laserColour;
		Mesh m_Mesh = null;

		[SerializeField]
		Material m_material = null;

		List<TargetCircle> m_circleList = new List<TargetCircle>();

		Quaternion m_rotation;

		float m_width, m_length;

		const int kTargetCircleSides = 32;
		const float lTargetCicleRadius = 1f;

		static float[] cache_cosine, cache_sine;

		public void CreateNewLaser(Color laserColour,Quaternion rotation, float width, float length)
		{
			lifeElapsed = 0;

			m_laserColour = laserColour;
			m_rotation = rotation;
			m_length = length;
			m_width = width;

			if(m_Mesh == null)
			{
				BuildMesh();
			}
		}

		public void ClearAll()
		{
			ClearLaser ();
			ClearCircles ();
		}

		public void ClearLaser()
		{
			lifeElapsed = kLifeTime + 1; //Add one to kLifetime so lifeElapsed will always go over;
		}

		public void ClearCircles()
		{
			m_circleList.Clear ();
		}

		void BuildMesh()
		{
			m_Mesh = new Mesh ();

			Vector3[] verts = new Vector3[4];
			Vector2[] uvs = new Vector2[4];
			int[] tris = new int[6]{0,1,2,2,1,3};
			Color[] vColours = new Color[4]{m_laserColour,m_laserColour,m_laserColour,m_laserColour};
			float halfWidth = m_width * 0.5f;

			verts [0] = new Vector3 (-halfWidth, 0, 0);
			verts [1] = new Vector3 (-halfWidth, 0, m_length);
			verts [2] = new Vector3 (halfWidth, 0, 0);
			verts [3] = new Vector3 (halfWidth, 0, m_length);

			m_Mesh.vertices = verts;
			m_Mesh.colors = vColours;
			m_Mesh.uv = uvs;
			m_Mesh.triangles = tris;
			m_Mesh.RecalculateNormals ();
		}

		void LateUpdate()
		{
			lifeElapsed += Time.deltaTime;
			if(lifeElapsed <= kLifeTime && m_Mesh != null && m_material != null)
			{
				Matrix4x4 matrix = Matrix4x4.identity;
				matrix.SetTRS(this.transform.position + Vector3.up * 0.01f,m_rotation,Vector3.one);
				Graphics.DrawMesh(m_Mesh,matrix,m_material,0);
			}
			if(m_circleList.Count > 0)
			{
				Matrix4x4 matrix =  Matrix4x4.identity;
				matrix.SetTRS(this.transform.position,Quaternion.identity,Vector3.one);
				for(int i = 0; i < m_circleList.Count; i++)
				{
					if(m_circleList[0] != null)
						Graphics.DrawMesh(m_circleList[0].GetMesh(), matrix,m_material,0);
				}
			}
		}

		public IEnumerator DrawTargetCircle(Color c, float life)
		{
			if(m_material != null)
			{
			//Reclaim resources from target circle as soon as we can. Maybe move this class into an object pool pattern later?
				using(TargetCircle targetCircle = new TargetCircle(c,life))
				{
					m_circleList.Add(targetCircle);
					while(!targetCircle.AwaitingDisposal())
					{
						targetCircle.UpdateAndPaint();
						yield return new WaitForEndOfFrame();
					}
					m_circleList.Remove(targetCircle);
				}
				yield break;
			}
		}

		class TargetCircle : System.IDisposable
		{
			Mesh m_mesh;
			Color m_color;

			float m_life, m_radius, m_alpha = 0;

			const int kSides = 16;
			const float kSpeed = 1f, kWidth = 0.5f,kEndRadius = 1f;

			public float borderRadius{get{return m_life + kWidth + kEndRadius;}}

			static readonly float[] cosine, sine;

			public TargetCircle(Color color, float displayFor)
			{
				m_mesh = new Mesh();
				m_color = color;
				m_life = displayFor;
			}

			//Static constructor.
			static TargetCircle()
			{
				float angle = 360f / kSides;

				sine = new float[kSides];
				for(int i = 0; i < kSides; i++)
				{
					sine[i] = Mathf.Sin(angle * i * Mathf.Deg2Rad);
				}

				cosine = new float[kSides];
				for(int i = 0; i < kSides; i++)
				{
					cosine[i] = Mathf.Cos(angle * i * Mathf.Deg2Rad);
				}
			}

			public void UpdateAndPaint()
			{
				m_life -= Time.deltaTime * kSpeed;
				BuildMesh ();
				m_alpha = Mathf.Min (m_alpha + Time.deltaTime, 1);
			}

			void BuildMesh()
			{
				Color alphaColor = new Color(m_color.r,m_color.g,m_color.b,m_alpha);
				MeshGeneration.CircularOutline (m_mesh, kSides,m_life + kEndRadius,borderRadius,m_color,cosine,sine,HideFlags.HideAndDontSave);
			}

			public Mesh GetMesh()
			{
				return m_mesh;
			}

			public bool AwaitingDisposal()
			{
				return m_life <= 0;
			}

			public void Dispose()
			{
				DestroyObject (m_mesh);
			}
		}
	}
}
