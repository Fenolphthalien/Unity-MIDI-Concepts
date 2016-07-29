using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    namespace Debugging
    {
        public class BezierVisualiser : MonoBehaviour
        {
            public QuadraticBezier quadraticBezier;

            public int resolution = 10;

            void OnDrawGizmos()
            {
                resolution = resolution > 2 ? resolution : 2;
                float div = 1f / (resolution - 1);
                Vector3 lastPosition = quadraticBezier.Evaluate(0) + transform.position;
                Vector3 position;
                for (int i = 1; i < resolution; i++)
                {
                    position = quadraticBezier.Evaluate(div * i) + transform.position;
                    Gizmos.DrawLine(lastPosition, position);
                    lastPosition = position;
                }
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(quadraticBezier.p0 + transform.position, quadraticBezier.p1 + transform.position);
                Gizmos.DrawLine(quadraticBezier.p1 + transform.position, quadraticBezier.p2 + transform.position);
                Gizmos.color = Color.white;

                Gizmos.DrawWireSphere(quadraticBezier.p0 + transform.position, 0.25f);
                Gizmos.DrawWireSphere(quadraticBezier.p1 + transform.position, 0.25f);
                Gizmos.DrawWireSphere(quadraticBezier.p2 + transform.position, 0.25f);
            }
        }
    }
}
