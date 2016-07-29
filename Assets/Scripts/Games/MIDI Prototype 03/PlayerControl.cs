using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace PrototypeThree
{
	public class PlayerControl : MonoBehaviour, IContainObjects
	{
		FloorControl m_floor;

		const float kFireRate = 0.14f;

		[SerializeField]
		GameObject m_projectilePrefab; // Assigned in inspector.

		List<GameObject> m_projectileList =  new List<GameObject>();

		int m_health, m_l_health;

		float m_fireWait = 0;

		public bool damageTaken{get{return m_health != m_l_health;}}
		public bool playerKilled{get{return damageTaken && m_health <= 0;}}

		public void Initialise(FloorControl floor)
		{
			if(floor != null)
			{
				m_floor = floor;
			}
			//TODO: Throw exception if null.
			m_health = 100;
			m_l_health = m_health;
		}

		public void Clear()
		{
			for(int i = 0; i < m_projectileList.Count; i++)
			{
				if(m_projectileList[i] != null)
					Destroy(m_projectileList[i]);
			}
			m_projectileList.Clear ();
		}

		public int GetHealth()
		{
			return m_health;
		}

		void Update()
		{
			//Move left
			if(Input.GetKey(KeyCode.A))
			{
				if(LeftBound() > -Constants.kScreenBound)
				{
					Vector3 position = transform.position + Vector3.left * Time.deltaTime * Constants.playerSpeed, moveBy;
					if(m_floor.AttemptMove(transform.position,position, out moveBy))
						transform.Translate(moveBy,Space.World);
				}
			}
			if(Input.GetKey(KeyCode.D))
			{
				if(RightBound() < Constants.kScreenBound)
				{
					Vector3 position = transform.position + Vector3.right * Time.deltaTime * Constants.playerSpeed, moveBy;
					if(m_floor.AttemptMove(transform.position,position, out moveBy))
						transform.Translate(moveBy,Space.World);
				}
			}
			if(Input.GetKey(KeyCode.Space) && m_fireWait <= 0f)
			{
				SpawnProjectile();
			}
			m_fireWait = m_fireWait - Time.deltaTime > 0f ? m_fireWait - Time.deltaTime : 0f;
		}

		void LateUpdate()
		{
			m_l_health = m_health;
		}

		float RightBound()
		{
			return transform.position.x + transform.localScale.x;
		}

		float LeftBound()
		{
			return transform.position.x - transform.localScale.x;
		}

		void SpawnProjectile()
		{
			if (m_projectilePrefab != null) 
			{
				GameObject gameObject =  Instantiate<GameObject> (m_projectilePrefab);
				gameObject.transform.position = transform.position + Vector3.up;
				m_fireWait = kFireRate;
				m_projectileList.Add(gameObject);
			}
		}

		void OnTriggerEnter(Collider c)
		{
			Block block = c.GetComponent<Block> ();
			if(block != null)
			{
				block.PerformCollision(null);
				m_health -= 5;
			}
		}
	}
}
