using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class MoveHorizontallyDependant : MonoBehaviour, IHorizontalMovementHandler
    {
        public Constraint clamp;
        public bool constrain;

        public virtual void HandleHorizontalMovement(float delta)
        {
            if (!enabled)
            {
                return;
            }
            if(constrain && !clamp.InConstraint(transform.position.x + delta))
            {
                return;
            }
            transform.Translate(Vector3.right * delta, Space.World);
        }
        
        protected void OnDrawGizmosSelected()
        {
            if (constrain)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(Vector3.right * clamp.max, Vector3.one);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(Vector3.right * clamp.min, Vector3.one);
            }
        }
    }
}
