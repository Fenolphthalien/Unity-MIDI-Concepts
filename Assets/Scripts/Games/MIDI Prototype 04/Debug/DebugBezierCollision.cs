using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [RequireComponent(typeof(MeshCollider), typeof(CubicBezierRenderer))]
    public class DebugBezierCollision : MonoBehaviour
    {
        void OnCollisionStay(Collision C)
        {
            Debug.Log("Collider entered");
        }
    }
}
