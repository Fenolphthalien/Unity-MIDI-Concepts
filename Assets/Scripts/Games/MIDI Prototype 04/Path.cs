using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [System.Serializable]
    public class PingPongPath
    {
        public Vector3 A, B;

        [SerializeField]
        Transform m_parent;

        public PingPongPath(Vector3 a, Vector3 b, Transform parent)
        {
            A = a;
            B = b;
            m_parent = parent;
        }

        public Vector3 Evaluate(float t, bool invert = false)
        {
            Vector3 a = invert ? B : A, b = invert ? A: B;
            float remainder = t % 1;
            Vector3 offset = m_parent != null ? m_parent.position : Vector3.zero;
            return Vector3.Lerp(a + offset, b + offset, remainder);
        }

        public float DistanceBetweenAB()
        {
            return Vector3.Distance(A, B);
        }
    }
}
