using UnityEngine;
using System.Collections;

namespace PrototypeThree
{
	[RequireComponent(typeof(Collider))]
	public class Projectile : MonoBehaviour
	{
		const float kSpeed = 25, lifeTime = 2f;

		float m_lifeElapsed = lifeTime;

		[SerializeField]
		int m_damage = 80;

		void Update () 
		{
			transform.Translate (Vector3.up * Time.deltaTime * kSpeed);
			if(m_lifeElapsed <= 0)
			{
				Destroy(this.gameObject);
				return;
			}
			m_lifeElapsed -= Time.deltaTime;
		}

		public int GetDamage()
		{
			return m_damage;
		}
	}
}
