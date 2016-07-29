using UnityEngine;
using System.Collections;

namespace NOsu
{
	[RequireComponent(typeof(SpriteRenderer),typeof(SphereCollider))]
	public sealed class NosuEmitter : MonoBehaviour, IFireAtTarget, IPrepareToFire,IDamageable
	{
		bool  m_broke;

		const float kLaserWidth = 1f, kLaserHeight = 6.77f;

		[SerializeField]
		NosuEmmiterRenderer m_emmiterRenderer = null;

		float m_health = 100;

		SpriteRenderer m_spriteRenderer = null;

		Color m_colour = Color.white;

		//=================================
		// Public functions
		//=================================
		
		public void Initialise(Vector3 startPosition)
		{
			this.transform.position = startPosition;
			m_broke = false;
			m_health = 100;
			HealthChanged ();
			m_spriteRenderer = this.GetComponent<SpriteRenderer> ();
			StopAllCoroutines ();
			m_emmiterRenderer.ClearAll ();
		}

		public void Initialise(Vector3 startPosition, Color C)
		{
			Initialise (startPosition);
			m_colour = C;
		}
		
		public void Swap(NosuEmitter other)
		{
			Vector3 positionCopy = other.transform.position;
			other.transform.position = this.transform.position;
			this.transform.position = positionCopy;
		}

		public void FireAtTarget(IDamageable target)
		{
			if(!m_broke && target.DamageCheck (new DamageArgs(this,5,EIgnoreDamage.EMMITER))) // Attempt to damage target
			{
				Vector3 local = target.GetPosition() - transform.position;
				float angle = Mathf.Atan2(local.x,local.z) * Mathf.Rad2Deg; // Calculate angle.
				m_emmiterRenderer.CreateNewLaser (m_colour,Quaternion.Euler(Vector3.up*angle),kLaserWidth,kLaserHeight);
			}
		}
		
		public bool DamageCheck(DamageArgs args)
		{
			Projectile projectile = (Projectile)args.sender;
			if(projectile != null)
			{
				projectile.Destroy();
			}
			if(!m_broke && args.ignore != EIgnoreDamage.EMMITER)
			{
				DamageRecieved(args.damage);
				return true;
			}
			return false;
		}

		public Vector3 GetPosition()
		{
			return transform.position;
		}

		public void PrepareToFire(IDamageable target)
		{
			if(!m_broke)
				StartCoroutine (ShowFireCircleThenFire (target));
		}

		public DamageArgs GetArgs()
		{
			return new DamageArgs (this, 5, EIgnoreDamage.EMMITER);
		}

		public void Clear()
		{
			m_emmiterRenderer.ClearAll ();
		}

		public void Clear(bool m_laser, bool circles)
		{
			if (m_laser)
				m_emmiterRenderer.ClearLaser ();
			if (m_laser)
				m_emmiterRenderer.ClearCircles ();
		}

		//=================================
		// Mono functions
		//=================================


		void Update()
		{
			if (!m_broke && m_health < 100) 
			{
				RestoreHealth();
			}
		}

		void OnTriggerEnter(Collider c)
		{
			IDamaging damaging = c.GetComponent<IDamaging> ();
			if(damaging != null)
			{
				DamageCheck(damaging.GetArgs());
			}
		}

		//=================================
		// Private functions
		//=================================

		void DamageRecieved(int amount)
		{
			m_health -= amount;
			HealthChanged ();
			if(m_health <= 0)
			{
				StartCoroutine(BreakEmitter());
			}
		}

		void RestoreHealth()
		{
			m_health = Mathf.Min(m_health + 100 * Time.deltaTime,100);
			HealthChanged ();
		}

		void HealthChanged()
		{
			if(m_spriteRenderer != null)
			{
				m_spriteRenderer.color = new Color(m_health * 0.01f,m_health * 0.01f,m_health*  0.01f,1);
			}
		}

		//=================================
		// Coroutines
		//=================================

		IEnumerator ShowFireCircleThenFire(IDamageable target)
		{
			yield return StartCoroutine(m_emmiterRenderer.DrawTargetCircle (m_colour, 1f));
			FireAtTarget (target);
		}

		IEnumerator BreakEmitter()
		{
			m_broke = true;
			m_emmiterRenderer.ClearCircles ();
			yield return new WaitForSeconds (2f);
			while(m_health < 100)
			{
				RestoreHealth();
				yield return new WaitForEndOfFrame();
			}
			m_broke = false;
			yield break;
		}
	}
}
