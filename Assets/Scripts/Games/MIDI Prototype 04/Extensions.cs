using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public static class Extensions
    {
        public static void AddCubicBezierComponents(this GameObject go, CubicBezier bezier,Material bezierMat)
        {
            if (bezier == null)
                return;

            bezier.Compute(true);

            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = bezier.GetMesh();
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = bezierMat;
            MeshCollider mc = go.AddComponent<MeshCollider>();
            bezier.AssignMeshAsCollider(mc, false);
        }
    }
}
