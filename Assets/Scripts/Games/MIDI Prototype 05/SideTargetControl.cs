using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class SideTargetControl : TargetControlBase
    {
        public int rows = 1;
        public int columns = 3;
        public float xSpacing = 1, ySpacing = 1;

        [ContextMenu("Populate")]
        protected override void Populate()
        {
            int i = 0;
            if (targetPrefab == null)
                return;
            
            Depopulate();

            rows = rows > 0 ? rows : 1;
            columns = columns > 0 ? columns : 1;
            int length = rows * columns, c = 0, r = 0, cinc = 1;
            Vector3[] points = new Vector3[length];
            Vector3 position;
            GameObject go;
            objects = new Target[length];

            for (i = 0; i < length; i++)
            {
                position = (Vector3.right * (c * xSpacing)) + Vector3.up * (ySpacing * r);
                position = transform.TransformPoint(position);
                points[i] = position;

                go = Instantiate(targetPrefab);
                go.transform.position = position;
                go.transform.rotation = this.transform.rotation;
                go.transform.SetParent(this.transform);
                Target st = go.GetComponent<Target>();
                if(st != null)
                 objects[i] = st;

                if (c >= columns - 1 && cinc > 0)
                {
                    cinc = -1;
                    r++;
                }
                else if (c <= 0 && cinc < 0)
                {
                    cinc = 1;
                    r++;
                }
                else
                {
                    c += cinc;
                }
            }

        }

        [ContextMenu("Depopulate")]
        protected override void Depopulate()
        {
            int i;
            if (objects != null)
            {
                for (i = 0; i < objects.Length; i++)
                {
                    DestroyImmediate(objects[i]);
                    objects[i] = null;
                }
                objects = null;
            }
        }

        protected override void OnDrawGizmos()
        {
            rows = rows > 0 ? rows : 1;
            columns = columns > 0 ? columns : 1;
            int length = rows * columns, c = 0, r = 0, cinc = 1, i = 0;
            Vector3[] points = new Vector3[length];
            Vector3 position;

            Gizmos.matrix = transform.localToWorldMatrix;
            float rgb = 1f/length;

            for (i = 0; i < length; i++)
            {
                Gizmos.color = new Color(1, rgb * i, rgb * i, gizmoOpacity);
                position = (Vector3.right * (c * xSpacing)) + Vector3.up * (ySpacing * r);
                if (gizmoMesh == null)
                    Gizmos.DrawWireCube(position + Vector3.up * 0.5f, Vector3.one);
                else
                    Gizmos.DrawWireMesh(gizmoMesh, position);
                position = transform.TransformPoint(position);
                points[i] = position;
                if (c >= columns - 1 && cinc > 0)
                {
                    cinc = -1;
                    r++;
                }
                else if (c <= 0 && cinc < 0)
                {
                    cinc = 1;
                    r++;
                }
                else
                {
                    c += cinc;
                }
            }

            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.identity;

            for (i = 1; i < length; i++)
            {
                Gizmos.DrawLine(points[i - 1], points[i]);
            }
        }
    }
}