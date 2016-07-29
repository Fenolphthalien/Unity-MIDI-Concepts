using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    public class CubicBezierRenderer : MonoBehaviour
    {
        const int resolution = 18;
        const float increment = 1f / resolution;

        public float gizmoSize = 1, handle = 0.2f, width = 1f;

        public bool DrawGizmos;

        public Vector3 A, B;

        Mesh m_mesh;

        void Awake()
        {
            OnDrawGizmos();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.grey;
            Vector3 lA = Vector3.zero, lB = B - A;
            Vector3[] points = new Vector3[resolution], perpVector = new Vector3[resolution];
            float x = lB.x * handle;
            Vector3 p0 = lA, p1 = new Vector3(x, 0, 0), p2 = new Vector3(lB.x - x, lB.y, 0), p3 = lB, perp0, perp1;
            Vector3 derivative;
            for (int i = 0; i < resolution; i++)
            {
                points[i] = CubicPoint(p0, p1, p2, p3, i * increment);
                if (DrawGizmos)
                {
                    Gizmos.DrawWireSphere(points[i], gizmoSize);
                    if (i > 0)
                    {
                        Gizmos.DrawLine(points[i - 1], points[i]);
                    }
                }
                derivative = CubicDerivative(p0, p1, p2, p3, i * increment);
                derivative = Vector3.Normalize(derivative);
                derivative = new Vector3(-derivative.y, derivative.x, 0); //take the derivative and rotate it -90 degrees.
                perpVector[i] = derivative;
                perpVector[i] *= (width * 0.5f);
                perp0 = points[i] + perpVector[i];
                perp1 = points[i] - perpVector[i];
                if (DrawGizmos)
                    Gizmos.DrawLine(perp0, perp1);
            }
            BuildMesh(ref m_mesh, ref points, ref perpVector);
            Gizmos.color = Color.blue;
            if (m_mesh != null)
            {
                MeshFilter mr = GetComponent<MeshFilter>();
                if (mr != null)
                    mr.sharedMesh = m_mesh;
            }
            AssignMeshAsCollider(false);
        }

        /* http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/ */
        Vector3 CubicPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float ct = (t >= 0 ? t : 0) <= 1 ? t : 1; //0 <= t <= 1
            return ((1 - ct) * (1 - ct) * (1 - ct)) * p0 + 3 * ((1 - ct) * (1 - ct)) * ct * p1 + 3 * (1 - ct) * (ct * ct) * p2 + (ct * ct * ct) * p3; //[x,y]=(1–t)3P0+3(1–t)2tP1+3(1–t)t2P2+t3P3
        }

        /* http://stackoverflow.com/questions/4089443/find-the-tangent-of-a-point-on-a-cubic-bezier-curve-on-an-iphone */
        Vector3 CubicDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float ct = (t >= 0 ? t : 0) <= 1 ? t : 1; //0 <= t <= 1
            return -3 * ((1 - t) * (1 - t)) * p0 + 3 * ((1 - t) * (1 - t)) * p1 - 6 * t * (1 - t) * p1 - ((3 * t) * (3 * t)) * p2 + 6 * t * (1 - t) * p2 + ((3 * t) * (3 * t)) * p3; //dP(t) / dt =  -3(1-t)^2 * P0 + 3(1-t)^2 * P1 - 6t(1-t) * P1 - 3t^2 * P2 + 6t(1-t) * P2 + 3t^2 * P3
        }

        void BuildMesh(ref Mesh mesh, ref Vector3[] points, ref Vector3[] derivatives)
        {
            if (points.Length != derivatives.Length)
                return;

            int length = points.Length, quad = 0, triArray = 0, vertIt = 0;
            Vector3 p0, p1, p2, p3;
            Vector3[] verts = new Vector3[length * 2];
            int[] tris = new int[verts.Length * 3];

            p2 = points[0] + derivatives[0];
            p3 = points[0] - derivatives[0];
            verts[0] = p2;
            verts[1] = p3;
            vertIt += 2;
            for (int i = 1; i < length; i++)
            {
                p0 = p2;
                p1 = p3;
                p2 = points[i] + derivatives[i];
                p3 = points[i] - derivatives[i];
                verts[vertIt] = p2;
                verts[vertIt + 1] = p3;
                vertIt += 2;
                AddQuadTri(ref quad, ref tris, ref triArray);
            }

            if (mesh != null)
                mesh.Clear();
            else
                mesh = new Mesh();
            mesh.hideFlags = HideFlags.HideAndDontSave;
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        void AddQuadTri(ref int quad, ref int[] triangleArray, ref int iterator)
        {
            triangleArray[iterator] = quad;
            triangleArray[iterator + 1] = quad + 2;
            triangleArray[iterator + 2] = quad + 1;
            triangleArray[iterator + 3] = quad + 2;
            triangleArray[iterator + 4] = quad + 3;
            triangleArray[iterator + 5] = quad + 1;
            iterator += 6;
            quad += 2;
        }

        void ExtrudeQuad(Vector3 extrudeDir, ref Vector3[] destVertexBuffer, ref int[] destTriBuffer, Vector3[] verts, int[] tris, ref int sourceVertIt, ref int sourceTriIt)
        {
            destVertexBuffer = new Vector3[8];
            destTriBuffer = new int[24];

            destVertexBuffer[0] = verts[sourceVertIt];
            destTriBuffer[0] = tris[sourceTriIt];

            destVertexBuffer[1] = verts[sourceVertIt + 1];
            destTriBuffer[1] = tris[sourceTriIt];

            destVertexBuffer[2] = verts[sourceVertIt + 2];
            destTriBuffer[2] = tris[sourceTriIt + 2];

            destVertexBuffer[3] = verts[sourceVertIt + 3];
            destTriBuffer[3] = tris[sourceTriIt + 3];

            destTriBuffer[4] = tris[sourceTriIt + 4];
            destTriBuffer[5] = tris[sourceTriIt + 5];
            sourceVertIt += 4;
            sourceTriIt += 6;
        }

        void AssignMeshAsCollider(bool trigger)
        {
            MeshCollider C = GetComponent<MeshCollider>();
            if (C == null)
                return;
            C.sharedMesh = m_mesh;
            C.isTrigger = trigger;
        }
    }
}