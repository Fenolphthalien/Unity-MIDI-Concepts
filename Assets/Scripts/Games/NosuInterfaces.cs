using UnityEngine;
using System.Collections;

namespace NOsu
{
	public interface IDamageable
	{
		bool DamageCheck(DamageArgs args);
		Vector3 GetPosition();
	}

	public interface IDamaging
	{
		DamageArgs GetArgs();
	}

	public interface IFire : IDamaging
	{
		void Fire();
	}

	public interface IFireAtTarget : IDamaging
	{
		void FireAtTarget(IDamageable target);
	}

	public interface IPrepareToFire : IDamaging
	{
		void PrepareToFire (IDamageable target);
	}

	public struct DamageArgs
	{
		public IDamaging sender;
		public int damage;
		public EIgnoreDamage ignore;

		public DamageArgs(IDamaging _sender, int _damage, EIgnoreDamage _ignore)
		{
			sender = _sender;
			damage = _damage;
			ignore = _ignore;
		}
	}
}


