using UnityEngine;
using System.Collections;

namespace NOsu{
	public class Projectile : MonoBehaviour, IDamaging
	{
		public float lifespan;
		public float speed = 1;
		float lifeElapsed;

		public int damage;
		public EIgnoreDamage ignore;
		
		void Update () 
		{
			transform.Translate (Vector3.forward * Time.deltaTime * speed, Space.Self);
			lifeElapsed += Time.deltaTime;
			if(lifeElapsed > lifespan)
			{
				Destroy (this.gameObject);
			}
		}

		public void Destroy()
		{
			Destroy (this.gameObject);
		}

		public void Destroy(out int _damage)
		{
			_damage = damage;
			Destroy (this.gameObject);
		}

		public DamageArgs GetArgs()
		{
			return new DamageArgs (this, damage, ignore);
		}
	}
}
