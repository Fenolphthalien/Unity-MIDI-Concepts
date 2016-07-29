using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [RequireComponent(typeof(Collider))]
    public class NotePellet : MonoBehaviour, IRequireDependancy<INotePelletContainer>
    {
        enum ETeleport { NONE, UP, DOWN };

        INotePelletContainer m_container;
        [SerializeField]
        protected float m_snapRadius = 1;

        protected bool bTeleportPlayer = false;

        Vector3 m_TeleportTarget;

        public void InjectDependancy(INotePelletContainer container)
        {
            m_container = container;
        }

        void SetTeleportationMaterial()
        {
            ETeleport teleportCheck = ETeleport.NONE;
            if (bTeleportPlayer)
            {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                MaterialCollection mc = GetComponent<MaterialCollection>();
                teleportCheck = m_TeleportTarget.y > transform.position.y ? ETeleport.UP : ETeleport.DOWN;
                mr.sharedMaterial = mc.GetMaterial((int)teleportCheck);
            }
        }

        protected virtual void OnTriggerEnter(Collider c)
        {
            Player player = c.GetComponent<Player>() as Player;
            if (player != null)
                PlayerCollided(player);
        }

        protected virtual void PlayerCollided(Player p)
        {
            p.AddShots(1);
            if (bTeleportPlayer)
            {
                p.Teleport(m_TeleportTarget);
            }
            RemoveSelfAndDestroy();
        }

        protected void RemoveSelfAndDestroy()
        {
            if (m_container != null)
            {
                GameObject gameObject = m_container.RemoveNotePellet(this); //Should return reference to this.gameObject if succesfull. This object should only be able to destroy itself.
                if (gameObject != null && gameObject == this.gameObject)
                    Destroy(gameObject);
            }
        }

        public void SetAllowTeleportWithDestination(bool bAllowTeleport, Vector3 destination)
        {
            bTeleportPlayer = bAllowTeleport;
            m_TeleportTarget = destination;
            SetTeleportationMaterial();
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = bTeleportPlayer ? Color.yellow : Color.red;
            Gizmos.DrawWireSphere(transform.position, m_snapRadius);
            if (bTeleportPlayer)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, m_TeleportTarget);
                Gizmos.DrawWireCube(m_TeleportTarget,Vector3.one*0.5f);
            }
        }

        public virtual bool InSnapBounds(Vector3 position, ref Vector3 snapTarget)
        {
            float m_snapSq = m_snapRadius * m_snapRadius;
            Vector3 difference = position - this.transform.position;
            float distanceSq = (difference.x * difference.x) + (difference.y * difference.y);
            snapTarget = this.transform.position;
            return distanceSq <= m_snapSq; 
        }
    }
}
