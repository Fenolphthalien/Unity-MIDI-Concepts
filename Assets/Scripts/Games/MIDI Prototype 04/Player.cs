using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [RequireComponent(typeof(Collider),typeof(Rigidbody))]
    public class Player : MonoBehaviour, IMoveToDestination, IRequireCollider, IRequireSingleIntialisation<Vector3>
    {
        public float speed = 1;
        float m_shots = 0;

        public TeleportCallback OnTeleport;

        Collider m_collider;

        void OnDestroy()
        {
            OnTeleport = null;
        }

        public void MoveTo(Vector3 destination, float delta, Constraint constraint)
        {
            float distance = destination.y - transform.position.y;
            float distanceAbs = distance < 0 ? -distance : distance;
            float overshoot = 0;
            Vector3 deltaPosition = Vector3.zero;
            if (distanceAbs > delta * speed)
            {
                deltaPosition = transform.position + Mathf.Sign(distance) * Vector3.up * (delta * speed);
                if (constraint.InConstraint(deltaPosition.y, out overshoot))
                {
                    transform.position = deltaPosition;
                    return;
                }
                deltaPosition -= overshoot * Vector3.up;
                transform.position = deltaPosition;
                return;
            }
            deltaPosition = transform.position + distance * Vector3.up;
            if (constraint.InConstraint(deltaPosition.y, out overshoot))
            {
                transform.position = deltaPosition;
                return;
            }
            deltaPosition -= overshoot * Vector3.up;
            transform.position = deltaPosition;
            return;
        }

        public int PeekAtShots()
        {
            return Mathf.FloorToInt(m_shots);
        }

        public int ReleaseShots()
        {
            int shots = Mathf.FloorToInt(m_shots);
            m_shots = 0;
            return shots;
        }

        public void AddShots(float amount)
        {
            m_shots += amount;
        }

        public Bounds GetBounds()
        {
            if (m_collider == null)
                m_collider = this.GetComponent<Collider>();
            return m_collider != null ? m_collider.bounds : new Bounds(Vector3.zero,Vector3.one);
        }

        public void Initialise(Vector3 parametre)
        {
            transform.position = parametre;
            m_shots = 0;
        }

        public void Teleport(Vector3 destination)
        {
            Vector3 oldPos = transform.position;
            transform.position = destination;
            if (OnTeleport != null)
            {
                Vector3 delta = transform.position - oldPos;
                OnTeleport.Invoke(destination, delta);
            }
        }
    }
}
