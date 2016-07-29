using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class CenterTargetControl : TargetControlBase
    {
        public Vector3[] spawnPoints;

        [ContextMenu("Populate")]
        protected override void Populate()
        {
            if (spawnPoints == null || targetPrefab == null)
                return;
            if (objects != null)
                Depopulate();
            objects = new Target[spawnPoints.Length];
            GameObject go;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                go = Instantiate(targetPrefab);
                Target t = go.GetComponent<Target>();
                if (t == null)
                {
                    DestroyImmediate(go);
                    return;
                }
                Vector3 position = transform.TransformPoint(spawnPoints[i]);
                go.transform.position = position;
                go.transform.rotation = this.transform.rotation;
                go.transform.SetParent(this.transform);
                objects[i] = t;
            }
        }

        [ContextMenu("Depopulate")]
        protected override void Depopulate()
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if(objects[i] != null)
                    DestroyImmediate(objects[i].gameObject);
            }
            objects = null;
        }

        protected override void OnDrawGizmos()
        {
            if (spawnPoints == null)
                return;
            Vector3 position;
            int i;
            Gizmos.matrix = transform.localToWorldMatrix;
            int length = spawnPoints.Length;
            float rgb = 1f / length;

            for (i = 0; i < length; i++)
            {
                Gizmos.color = new Color(1, rgb * i, rgb * i, gizmoOpacity);
                position = spawnPoints[i];
                if (gizmoMesh == null)
                    Gizmos.DrawWireCube(position + Vector3.up * 0.5f, Vector3.one);
                else
                    Gizmos.DrawWireMesh(gizmoMesh, position);
            }

            Gizmos.color = Color.white;

            for (i = 1; i < length; i++)
            {
                Gizmos.DrawLine(spawnPoints[i - 1], spawnPoints[i]);
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
