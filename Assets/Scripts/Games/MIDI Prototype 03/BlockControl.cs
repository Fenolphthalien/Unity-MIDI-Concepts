using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PrototypeThree
{
	public class BlockControl : MonoBehaviour, IContainObjects
	{
		List<Block> m_blockList = new List<Block>();

		public int count{get{return m_blockList != null ? m_blockList.Count : 0;}}

		FloorControl m_floorControl;

		float m_floorHeight;

		public void Initialise(float floorHeight, FloorControl floorControl)
		{
			m_floorControl = floorControl;
			m_floorHeight = floorHeight;
			Clear ();
		}

		public void Update()
		{
			CheckBlocksForCollision ();
		}

		public void Add(Block block)
		{
			block.SetControl (this);
			m_blockList.Add (block);
		}

		public void Remove(Block block)
		{
			if (block.ControllerEqualTo (this))
				block.SetControl (null);
			m_blockList.Remove (block);
		}

		public void Clear()
		{
			for(int i = 0; i < m_blockList.Count; i++)
			{
				Destroy(m_blockList[i].gameObject);
			}
		}

		//Block Splitting currently does not work.

//		public void SplitBlock(Block block, float position)
//		{
//			Vector3 sourcePosition = block.transform.position;
//			if (position <= 0 || position >= 1)
//				return;
//
//			float width = block.GetWidth (), height = block.GetMeshHeight ();
//			float sourceXPosition = block.GetGlobalXPosition (), sourceStart = sourceXPosition - width * 0.5f;
//			float breakpoint = sourceStart + width * position;
//
//			float width01 = breakpoint - sourceStart;
//			float xPositive01 = breakpoint - sourcePosition.x;
//
//			//Replaces old mesh
//			Mesh mesh01 = new Mesh ();
//			Vector3[] verts = new Vector3[]
//			{
//				new Vector3 (sourceStart, -height * 0.5f, 0),
//				new Vector3 (sourceStart, height * 0.5f, 0),
//				new Vector3 (breakpoint, -height * 0.5f, 0),
//				new Vector3 (breakpoint, height * 0.5f, 0)
//			};
//			Vector2[] uvs = new Vector2[]
//			{
//				Vector2.zero,
//				Vector2.up,
//				Vector2.right,
//				Vector2.one
//			};
//
//			int[] triangles = new int[]
//			{
//				0,1,2,
//				2,1,3
//			};
//
//			mesh01.vertices = verts;
//			mesh01.uv = uvs;
//			mesh01.triangles = triangles;
//			mesh01.RecalculateNormals ();
//
//			block.Initialise (mesh01, block.GetMaterial (), width01, height, sourceStart,xPositive01, sourcePosition, 1f);
//		}

		void CheckBlocksForCollision()
		{
			if(m_blockList != null && m_blockList.Count > 0)
			{
				for(int i = 0; i < m_blockList.Count; i++)
				{
					if(m_blockList[i] == null)
					{
						m_blockList.Remove(m_blockList[i]);
						continue;
					}
					if(m_blockList[i].GetObjectHeight() <= m_floorHeight)
					{
						if(m_floorControl.CheckCollision(m_blockList[i]))
							m_blockList[i].PerformCollision(null);
					}
				}
			}
		}
	}
}
