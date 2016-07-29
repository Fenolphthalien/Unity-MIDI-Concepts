using UnityEngine;
using System.Collections;

namespace NOsu
{
	public static class NosuPlayer
	{
		public const int kMaxHealth = 100, kMinHealth = 0;
	}

	public sealed class NosuPlayerControl : MonoBehaviour, IDamageable
	{
		[SerializeField]
		NosuPlayerRenderer m_playerRenderer = null;

		[SerializeField]
		GameObject m_playerProjectile = null;

		int m_health = NosuPlayer.kMaxHealth;

		bool m_invicible = false, m_allowPlayerControl;

		public bool allowPlayerControl {get {return m_allowPlayerControl;}}
			
		Vector3 m_lastMousePosition;

		void Update () 
		{
			if(m_allowPlayerControl)
			{
				if(Input.mousePosition != m_lastMousePosition)
				{
					float distance;
					Ray ray;
					if(Raycast(out ray, out distance))
					{
						Quaternion rotation = Quaternion.LookRotation(ray.GetPoint(distance),new Vector3(0,1,0));
						transform.rotation = rotation;
					}
					m_lastMousePosition = Input.mousePosition;
				}
				if(Input.GetMouseButtonDown(0))
				{
					SpawnProjectile();
				}
			}
		}

		void OnEnable()
		{
			if (m_playerRenderer != null)
				m_playerRenderer.enabled = true;
		}

		void OnDisable()
		{
			if (m_playerRenderer != null)
				m_playerRenderer.enabled = false;
		}

		/** http://answers.unity3d.com/questions/616454/create-new-plane-and-raycast-in-c.html **/
		bool Raycast(out Ray ray, out float distance)
		{
			Plane plane = new Plane(Vector3.up,Vector3.zero); //Create upwards facing plane.
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			return plane.Raycast (ray, out distance);
		}

		public bool DamageCheck(DamageArgs args)
		{
			if (!m_invicible && args.ignore != EIgnoreDamage.PLAYER) 
			{
				m_health -= args.damage;
				m_playerRenderer.SetHealth(m_health);
				return true;
			}
			return false;
		}

		public bool IsPlayerDead()
		{
			return m_health <= NosuPlayer.kMinHealth;
		}

		public void FillHealth()
		{
			Debug.Log ("Filling Players health");
			m_health = NosuPlayer.kMaxHealth;
			m_playerRenderer.SetHealth (m_health);
		}

		public Vector3 GetPosition()
		{
			return m_playerRenderer != null ? transform.position : m_playerRenderer.transform.position;
		}

		public void EnablePlayerInput(bool b)
		{
			m_allowPlayerControl = b;
		}

		void SpawnProjectile()
		{
			GameObject projectile = (GameObject)Instantiate (m_playerProjectile);
			float distance;
			Ray ray;
			if(Raycast(out ray, out distance) && projectile != null)
			{
				Quaternion rotation = Quaternion.LookRotation(ray.GetPoint(distance),new Vector3(0,1,0));
				projectile.transform.rotation = rotation;
				projectile.transform.Translate(Vector3.forward + Vector3.up*0.01f,Space.Self);
			}
		}
	}
}
