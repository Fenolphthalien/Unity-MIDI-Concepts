using UnityEngine;
using System.Collections;

namespace PrototypeThree
{
	[RequireComponent(typeof(BoxCollider),typeof(MeshFilter),typeof(MeshRenderer))]
	public class Block : MonoBehaviour, ICollide
	{
		Mesh m_mesh;
		Material m_material;

		float m_halfHeight, m_gravity, m_width, m_negativeX, m_positiveX;

		const float kTerminalVelocity = 9.81f, kGravityAccelaration = 2f;
		const int kMaxHealth = 100;

		BlockControl m_blockControl;

		int m_health = kMaxHealth;

        public AudioClip damageSound_healthy, damageSound_nearDeath;

        public AudioClip damageSound { get { return m_health > 25 ? damageSound_healthy : damageSound_nearDeath; } }

		public void Initialise(Mesh mesh, Material material,float width, float height, float negativeX, float positiveX, Vector3 position)
		{
			m_mesh = mesh;
			m_material = material;

			m_width = width;
			m_negativeX = negativeX;
			m_positiveX = positiveX;

			transform.position = position;

			BoxCollider collider = GetComponent<BoxCollider> ();
			if (collider != null) 
			{
				collider.size = new Vector3(m_width, height, 1);
				collider.center = new Vector3(CenterOfCollision (m_negativeX, m_positiveX), 0, 0);
				collider.isTrigger = true;
			}

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

			if(meshFilter != null)
				meshFilter.mesh = mesh;
			if(meshRenderer != null)
				meshRenderer.material = material;

			m_halfHeight = height * 0.5f;
			//Perform rigid Body check
			Rigidbody rigidBody = GetComponent<Rigidbody> ();
			if(rigidBody == null)
				AddRigidBody();
		}

        public void Initialise(Mesh mesh, Material material, float width, float height, float negativeX, float positiveX, Vector3 position, AudioClip _damageSound_healthy, AudioClip _damageSound_nearDeath)
        {
            Initialise(mesh, material, width, height, negativeX, positiveX, position);
            damageSound_healthy = _damageSound_healthy;
            damageSound_nearDeath = _damageSound_healthy;
        }

		public void Initialise(Mesh mesh, Material material,float width, float height, float negativeX, float positiveX, Vector3 position, float propolsion)
		{
			Initialise (mesh, material,width, negativeX,positiveX, height, position);
			m_gravity = propolsion;
		}

		public void PerformCollision(ICollide collide)
		{
			Destroy (this.gameObject);
		}

		public void SetControl(BlockControl control)
		{
			m_blockControl = control;
		}

		public bool ControllerEqualTo(BlockControl control)
		{
			return control != null && m_blockControl != null && m_blockControl.Equals (control);
		}

		void Update()
		{
			transform.Translate (Vector3.down * Time.deltaTime * m_gravity, Space.World);
			m_gravity = m_gravity + Time.deltaTime * kGravityAccelaration >= kTerminalVelocity ? kTerminalVelocity : m_gravity + Time.deltaTime * kGravityAccelaration;
		}

		public float GetObjectHeight()
		{
			return transform.position.y - m_halfHeight;
		}

		public float GetMeshHeight()
		{
			return m_halfHeight * 2;
		}

		public float GetWidth()
		{
			return m_width;
		}

		public float GetGlobalXPosition()
		{
			return transform.position.x - CenterOfCollision(m_negativeX, m_positiveX);
		}

		public void GetXBounds(out float negativeX, out float positiveX)
		{
			negativeX = transform.position.x - m_negativeX;
			positiveX = transform.position.x + m_positiveX;
		}

		float CenterOfCollision(float xneg, float xpos)
		{
			float distance = xpos - xneg;
			return distance * 0.5f;
		}

		void CollideProjectile(Projectile projectile)
		{
			int baseDamage = projectile.GetDamage ();
			int totalDamage = m_width > 1f ? (int)(baseDamage / m_width) : baseDamage;
			m_health -= totalDamage;

            if(damageSound != null)
                AudioSource.PlayClipAtPoint(damageSound,this.transform.position,0.7f);

			Destroy (projectile.gameObject);
			if (m_health <= 0)
				Destroy (this.gameObject);
		}

		Rigidbody AddRigidBody()
		{
			Rigidbody rigidBody = this.gameObject.AddComponent<Rigidbody> ();
			if (rigidBody != null) {
				rigidBody.useGravity = false;
				rigidBody.constraints = RigidbodyConstraints.FreezeAll;
			}
			return rigidBody;
		}

		void OnTriggerEnter(Collider c)
		{
			Projectile projectile = c.GetComponent<Projectile> ();
            Debug.Log(projectile);
			if(projectile != null)
			{
				CollideProjectile(projectile);
				return;
			}
		}

		bool SplitCheck(out float position, float colliderXPosition)
		{
			if(m_width >= 1)
			{
				//Calculate impact position.
				float localColliderX = transform.position.x - colliderXPosition;

				if(localColliderX <= 0)
				{
					position = 0;
					return false;
				}
				else if(localColliderX >= m_width)
				{
					position = 1;
					return false;
				}
				Debug.Log(localColliderX);
				position = localColliderX / m_width;
				return true;
			}
			position = -1f;
			return false;
		}

		public int GetHealth()
		{
			return m_health;
		}

		public Material GetMaterial()
		{
			return m_material;
		}
	}
}
