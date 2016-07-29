using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    [System.Serializable]
    public class TargetedGizmo
    {
        public Mesh mesh;
        public Material material;

        Quaternion euler { get { return Quaternion.Euler(new Vector3(0, Time.time * 360, 0)); } }


        public void Draw(ITargetable targetable)
        {
            if (mesh == null || material == null)
                return;
            Vector3 offset;
            Matrix4x4 matrix = Matrix4x4.TRS(targetable.GetTransform(out offset).position + offset,euler,Vector3.one);
            Graphics.DrawMesh(mesh, matrix, material, 0);
        }
    }
}
