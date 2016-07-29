using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public sealed class NotePelletLong : NotePellet
    {
        bool m_entered;
        Player m_player;

        float m_percentageTraveled = 0, m_length;
        const float kBonusThreshold = 0.8f;

        Vector3 m_lastCollision;

        public void Initialise(Mesh mesh, Material mat,BoxCollider bc,float length, INotePelletContainer container = null)
        {
            m_length = length;
            if(container != null)
                InjectDependancy(container);
            bc.size = new Vector3(length, 1f, 1);
            bc.center = Vector3.right * (length * 0.5f);
            bc.isTrigger = true;
            MeshFilter mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = gameObject.AddComponent<MeshRenderer>();
            mr.material = mat;
        }

        protected override void OnTriggerEnter(Collider c)
        {
            m_player = c.GetComponent<Player>();
            if (m_player != null)
                m_entered = true;
        }

        void OnTriggerStay(Collider c)
        {
            if (m_entered)
            {
                if (m_player != null)
                {
                    float delta = m_player.transform.position.x - m_lastCollision.x;
                    delta = delta < 0 ? -delta : delta;
                    //Debug.Log(delta);
                    PlayerCollided(m_player);
                    m_lastCollision = m_player.transform.position;
                }
            }
        }

        public override bool InSnapBounds(Vector3 position, ref Vector3 snapTarget)
        {
            return false;
        }

        protected override void OnDrawGizmos(){}

        protected override void PlayerCollided(Player p)
        {
            p.AddShots(Time.deltaTime);
        }

        void OnTriggerExit(Collider c)
        {
            if (m_entered)
            {
                m_player = null;
                RemoveSelfAndDestroy();
            }
        }
    }
}
