using UnityEngine;
using System.Collections;

namespace PrototypeThree
{
	public class BlockPainter : MonoBehaviour
	{
		bool m_triggered, m_meshDirty;
		Mesh m_mesh;
		Material m_material;
		Color m_colour;

		const float kHalfHeight = 0.5f, kHalfWidth = 0;

		float m_xDelta, m_nxDelta, m_position, m_speed  = 1;
		Vector3 m_startedDrawingAt, m_startedAt;

		float m_startDrawX {get{return m_startedDrawingAt.x;}}

		public float startSinPosition;
        public bool triggered { get { return m_triggered; } }

        public AudioClip hitSound, hitSoundNearDeath;

		void Awake()
		{
			m_startedAt = transform.position;
		}

		public void Initialise(float delay)
		{
			m_position = startSinPosition - delay;
            m_triggered = false;
		}
		
		void Update ()
		{
			float m_sine = Mathf.Sin (m_position);
			transform.position = m_startedAt + Vector3.right * m_sine * Constants.kScreenBound;

			if(m_triggered)
			{
				Sweep(m_startDrawX,transform.position.x);
			}
			m_position += Time.deltaTime * m_speed;
		}

		void Sweep(float from, float to)
		{
			if(transform.position.x > m_startDrawX)
			{
				float local = to - from, l_xDelta = m_xDelta;
				m_xDelta = local > m_xDelta ? local : m_xDelta; // If greater local will replace delta value.
				m_meshDirty = m_xDelta != l_xDelta; // Compare the new delta value against the old.
			}
			else if(transform.position.x < m_startDrawX)
			{
				float local = from - to, l_nxDelta = m_nxDelta;
				m_nxDelta = local > m_nxDelta ? local : m_nxDelta; // If greater local will replace delta value.
				m_meshDirty = m_nxDelta != l_nxDelta;// Compare the new delta value against the old.
			}
		}

		void LateUpdate()
		{
			if(m_triggered)
			{
				if(m_meshDirty)
				{
					PaintMesh();
				}
				Matrix4x4 matrix = Matrix4x4.identity;
				matrix.SetTRS(m_startedDrawingAt,Quaternion.identity,Vector3.one);
				Graphics.DrawMesh(m_mesh,matrix,m_material,0);
			}
		}

		public void Trigger(Material mat, Color c)
		{
			m_triggered = true;
			m_material = mat;
			m_colour = c;
			m_startedDrawingAt = transform.position;

			m_xDelta = kHalfWidth;
			m_nxDelta = kHalfWidth;

			m_meshDirty = true;
		}

		public void Trigger(Material mat, Color c, float speed)
		{
			Trigger (mat, c);
			m_speed = speed;
		}

		void PaintMesh()
		{
			if (m_mesh == null)
				m_mesh = new Mesh ();
			m_mesh.hideFlags = HideFlags.HideAndDontSave;

			Vector3[] verts = new Vector3[4]
			{
				new Vector3(-m_nxDelta,kHalfHeight,0),
				new Vector3(m_xDelta,kHalfHeight,0),
				new Vector3(-m_nxDelta,-kHalfHeight,0),
				new Vector3(m_xDelta,-kHalfHeight,0)
			};

			Color[] colours = new Color[4];

			for(int i = 0; i < 4; i++)
			{
				colours[i] = m_colour;
			}

			Vector2[] uvs = new Vector2[4]
			{
				Vector2.zero,
				Vector2.up,
				Vector2.right,
				Vector2.one
			};

			int[] triangles = new int[6]
			{
				0,1,2,
				2,1,3
			};

			m_mesh.vertices = verts;
			m_mesh.colors = colours;
			m_mesh.uv = uvs;
			m_mesh.triangles = triangles;

			m_mesh.RecalculateNormals ();
			m_mesh.RecalculateBounds ();

			m_meshDirty = false;
		}

		public Mesh Release(out float width, out float height, out float nxDelta, out float xDelta, out Vector3 position)
		{
			Mesh out_mesh = m_mesh;
			m_mesh = null;
			height = kHalfHeight * 2;
			width = m_xDelta + m_nxDelta;
			position = m_startedDrawingAt;
			nxDelta =  m_nxDelta;
			xDelta = m_xDelta;
			m_triggered = false;

			return out_mesh;
		}
	}
}
