using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class MoveVerticallyToMousePosition : MonoBehaviour, IMoveToDestination
    {
        public float speed = 1;

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
    }
}
