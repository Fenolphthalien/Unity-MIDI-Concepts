using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
    [System.Serializable]
    public class CubicBezier
    {
        const int resolution = 18;
        const float increment = 1f / resolution;

        public float handle = 0.2f, width = 1f;

        public Vector3 A, B;

        Vector3[] points = new Vector3[resolution], derivatives = new Vector3[resolution];
        Mesh m_mesh;

        public CubicBezier(Vector3 _A, Vector3 _B)
        {
            A = _A;
            B = _B;
            Compute(false);
        }

        public void Compute(bool buildMesh)
        {
            Vector3 lA = Vector3.zero, lB = B - A;
            points = new Vector3[resolution];
            derivatives = new Vector3[resolution];
            float x = lB.x * handle;
            Vector3 p0 = lA, p1 = new Vector3(x, 0, 0), p2 = new Vector3(lB.x - x, lB.y, 0), p3 = lB, perp0, perp1;
            Vector3 derivative;
            for (int i = 0; i < resolution; i++)
            {
                points[i] = CubicPoint(p0, p1, p2, p3, i * increment);
                derivative = CubicDerivative(p0, p1, p2, p3, i * increment);
                derivative = Vector3.Normalize(derivative);
                derivative = new Vector3(-derivative.y, derivative.x, 0); //take the derivative and rotate it -90 degrees.
                derivatives[i] = derivative;
                derivatives[i] *= (width * 0.5f);
                perp0 = points[i] + derivatives[i];
                perp1 = points[i] - derivatives[i];
            }
            if (buildMesh)
                BuildMesh(ref m_mesh, ref points, ref derivatives);
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

        public void BuildMesh()
        {
            BuildMesh(ref m_mesh, ref points, ref derivatives);
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
            mesh.RecalculateNormals();
        }

        public Mesh GetMesh()
        {
            return m_mesh;
        }

        public void AssignMeshAsCollider(MeshCollider C, bool trigger)
        {
            C.sharedMesh = m_mesh;
            C.isTrigger = trigger;
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

        public Vector3 GetPoint(int point)
        {
            int clampedPoint = (point >= 0 ? point : 0) < points.Length ? point : points.Length - 1;
            return points[clampedPoint];
        }

        public Vector3 GetDerivative(int derivative)
        {
            int clampedDerivative = (derivative >= 0 ? derivative : 0) < derivatives.Length ? derivative : derivatives.Length - 1;
            return derivatives[clampedDerivative];
        }
    }
}