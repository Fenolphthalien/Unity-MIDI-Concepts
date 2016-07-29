using UnityEngine;
using System.Collections;

namespace PrototypeThree{

	public class FloorControl : MonoBehaviour
	{

		readonly static float[] divisions = new float[]{-8f,-7f,-6f,-5f,-4f,-3f,-2f,-1f,-0f,1f,2f,3f,4f,5f,6f,7f,8f};

		[SerializeField]
		FloorBlock[] m_floorBlock;

		public void Initialise()
		{
			if(m_floorBlock != null)
			{
				for(int i = 0; i < m_floorBlock.Length; i++)
				{
					m_floorBlock[i].Initialise();
				}
			}
		}

		public bool CheckCollision(Block block)
		{
			if(block != null)
			{
				float xNeg, xPos;
				block.GetXBounds(out xNeg, out xPos);
				int startDivider = GetDivision(xNeg, -1), endDivider = GetDivision(xPos,1);
				if(startDivider < 0)
					return false;
				if(startDivider >= divisions.Length)
					return false;

				if(!m_floorBlock[startDivider].isWalkable && !m_floorBlock[endDivider].isWalkable)
					return false;

				m_floorBlock[startDivider].ApplyDamage(25,startDivider);

				int tileDifference = endDivider - startDivider;
				if(tileDifference > 0)
				{
					for(int i  = 0; i < tileDifference; i++)
					{
						m_floorBlock[i + startDivider].ApplyDamage(25,i);
					}
				}
				if(startDivider != endDivider)
				{
					m_floorBlock[endDivider].ApplyDamage(25,endDivider);
				}
				return true;
			}
			return false;
		}

		public bool AttemptMove(Vector3 sourcePosition, Vector3 targetPosition, out Vector3 MoveBy)
		{
			int direction = (int)Mathf.Sign (targetPosition.x - sourcePosition.x);
			int sourceDivider = GetDivision (sourcePosition.x,direction);
			int targetDivider = GetDivision (targetPosition.x,direction);

			if(targetDivider != -1 && !m_floorBlock[targetDivider].isWalkable)
			{
				MoveBy = Vector3.zero;
				return false;
			}
			MoveBy = targetPosition - sourcePosition;
			return true;
		}

		//Is position between two points?
		int GetDivision(float position, int direction)
		{
			switch(direction)
			{
				//Moving Left
				case -1:
				for(int i = 0; i < divisions.Length - 1; i++)
				{
					float distance1 = divisions[i] - position, distance2 = divisions[i+1] - position;
					if(distance1 <= 0 && distance2 >= 0)
					{
						return i;
					}
				}
				return -1;
				//Moving right
				case 1:
					for(int i = 1; i < divisions.Length; i++)
					{
						float distance1 = divisions[i-1] - position, distance2 = divisions[i] - position;
						if(distance1 <= 0 && distance2 >= 0)
						{
							return i-1;
						}
					}
				return -1;
			}
			return -1;
		}

		[ContextMenu ("Fill Floor Block Array With Children")]
		void FillFloorBlocksWithChildren()
		{
			System.Collections.Generic.List<FloorBlock> blocks = new System.Collections.Generic.List<FloorBlock> ();

			FloorBlock[] validChildren = GetComponentsInChildren<FloorBlock> ();
			for(int i = 0; i < validChildren.Length; i++)
			{
				if(validChildren[i] != null && validChildren[i].gameObject != this.gameObject)
				{
					blocks.Add(validChildren[i]);
				}
			}
			validChildren = null;
			m_floorBlock = blocks.ToArray ();
			blocks.Clear();
			blocks = null;
		}

		public int[] GetAllBlocksHealth()
		{
			System.Collections.Generic.List<int> healthList= new System.Collections.Generic.List<int> ();
			for(int i = 0; i < m_floorBlock.Length; i++)
			{
				if(m_floorBlock[i] != null)
					healthList.Add(m_floorBlock[i].GetHealth());
			}
			return healthList.ToArray ();
		}

		public int GetTotalHealth()
		{
			int health = 0;
			for(int i = 0; i < m_floorBlock.Length; i++)
			{
				if(m_floorBlock[i] != null)
					health += m_floorBlock[i].GetHealth();
			}
			return health;
		}

		public int GetBlockHealth(int block)
		{
			if(m_floorBlock[block] != null)
			{
				return m_floorBlock[block].GetHealth();
			}
			return -1;
		}
	}
}
