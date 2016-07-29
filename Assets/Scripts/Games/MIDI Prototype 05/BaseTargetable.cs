using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public abstract class TargetableBase : MonoBehaviour , ITargetable
    {
        protected Bounds bounds;
        public float lifeElapsed = 0;

        public const int points = 100;

        protected virtual void Update()
        {
            lifeElapsed += Time.deltaTime;
        }

        public virtual Transform GetTransform(out Vector3 positionOffset)
        {
            positionOffset = Vector3.zero;
            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null)
            {
                float yoffset = mf.mesh.bounds.size.y * 0.5f;
                positionOffset.y += yoffset;
            }
            return transform;
        }

        public virtual void FireAt(){}
    }
}
