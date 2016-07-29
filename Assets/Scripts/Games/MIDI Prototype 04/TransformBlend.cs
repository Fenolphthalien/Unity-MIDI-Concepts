using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class TransformBlend : MonoBehaviour, ITransformBlend
    {
        public KeyTransform transform0, transform1;

        protected float m_lerp = 0;
        public float lerp { get { return m_lerp; } }

        public void SetBlend(float f)
        {
            float trueValue = (f >= 0 ? f : 0) <= 1 ? f : 1;
            m_lerp = trueValue;
            Vector3 trueTransform = Vector3.Lerp(transform0.position, transform1.position, m_lerp);
            Quaternion trueQuant = Quaternion.Euler(Vector3.Lerp(transform0.eularAngles, transform1.eularAngles, m_lerp));
            transform.localPosition = trueTransform;
            transform.localRotation = trueQuant;
        }

        void OnDrawGizmosSelected()
        {
            Vector3 position0, position1, direct0, direct1;
            direct0 = Quaternion.Euler(transform0.eularAngles) * Vector3.forward;
            direct1 = Quaternion.Euler(transform1.eularAngles) * Vector3.forward;
            Vector3 lerp = Vector3.Lerp(transform0.position, transform1.position, m_lerp);
            
            position0 = transform.position + transform0.position - lerp;
            position1 = transform.position + transform1.position - lerp;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position0, Vector3.one);
            Gizmos.DrawLine(position0, position0 + direct0);
            Gizmos.DrawWireCube(position0 + direct0, Vector3.one * 0.5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(position1, Vector3.one);
            Gizmos.DrawLine(position1, position1 + direct1);
            Gizmos.DrawWireCube(position1 + direct1, Vector3.one * 0.5f);
        }
    }

    [System.Serializable]
    public struct KeyTransform
    {
        public Vector3 position, eularAngles;
        public Space space;
    }
}
