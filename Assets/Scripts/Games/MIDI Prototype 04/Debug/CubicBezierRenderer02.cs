using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [ExecuteInEditMode]
    public class CubicBezierRenderer02 : MonoBehaviour
    {
        public CubicBezier bezier = new CubicBezier(Vector3.zero, Vector3.one);

        void OnEnable()
        {
            if (bezier != null)
            {
                bezier.Compute(false);
                bezier.BuildMesh();
            }
            MeshFilter mf = GetComponent<MeshFilter>();
            MeshCollider MC = GetComponent<MeshCollider>();
            Mesh mesh = bezier.GetMesh();
            if (MC != null)
                bezier.AssignMeshAsCollider(MC, false);
            if (mf != null)
                mf.sharedMesh = mesh;
        }
    }
}