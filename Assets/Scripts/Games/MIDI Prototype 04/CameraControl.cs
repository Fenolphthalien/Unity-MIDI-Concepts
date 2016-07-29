using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class CameraControl : MonoBehaviour, IRequireSingleIntialisation<Vector3>, IMoveToDestination
    {
        public float speed = 1;
        const float kMouseRegion = 0.2f;
       
        [SerializeField]
        TransformBlend m_transformBlend = null;

        public void SetTransformBlend(float Amount)
        {
            if (m_transformBlend == null)
                return;
            m_transformBlend.SetBlend(Amount);
        }

        public void Initialise(Vector3 parametre)
        {
            transform.position = parametre;
        }

        public void MoveTo(Vector3 destination, float delta, Constraint constraint)
        {
            float distance = destination.y - transform.position.y;
            float mousePosition = Input.mousePosition.y / Screen.height;
            
            if (distance < 0 && mousePosition > kMouseRegion)
                return;
            if (distance > 0 && mousePosition < 1f - kMouseRegion)
                return;

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
        }
    }
}
