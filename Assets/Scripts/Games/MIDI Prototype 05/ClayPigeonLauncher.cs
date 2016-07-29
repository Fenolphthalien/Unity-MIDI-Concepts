using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public class ClayPigeonLauncher : MonoBehaviour
    {
        public GameObject clayPigeonPrefab = null;

        const float launchPower = 10;
        int lastLaunch;

        //Gizmos
        [Header("Editor Only")]
        public Mesh gizmoMesh = null;

        const float arrowheadHalfWidth = 0.25f, arrowHeadStart = 0.6f;

        public bool TryLaunch(out ClayPigeon pigeon)
        {
            pigeon = null;

            if (clayPigeonPrefab == null)
            {
                return false;
            }

            pigeon = Instantiate(clayPigeonPrefab).GetComponent<ClayPigeon>();
            if (pigeon == null)
                return false;
            Vector3 spawnAt = transform.TransformPoint(0, 0, 1);
            pigeon.transform.position = spawnAt;
            pigeon.forwardVector = transform.TransformVector(Vector3.forward) * launchPower;
            return true;
        }
        void OnDrawGizmos()
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;

            Vector3 A = new Vector3(arrowheadHalfWidth, arrowheadHalfWidth, arrowHeadStart);
            Vector3 B = new Vector3(-arrowheadHalfWidth, arrowheadHalfWidth, arrowHeadStart);
            Vector3 C = new Vector3(arrowheadHalfWidth, -arrowheadHalfWidth, arrowHeadStart);
            Vector3 D = new Vector3(-arrowheadHalfWidth, -arrowheadHalfWidth, arrowHeadStart);

            Gizmos.DrawLine(Vector3.zero, Vector3.forward);
            Gizmos.DrawLine(A, Vector3.forward);
            Gizmos.DrawLine(B, Vector3.forward);
            Gizmos.DrawLine(C, Vector3.forward);
            Gizmos.DrawLine(D, Vector3.forward);

            Gizmos.DrawLine(A, B);
            Gizmos.DrawLine(B, D);
            Gizmos.DrawLine(D, C);
            Gizmos.DrawLine(C, A);

            if (gizmoMesh == null)
            {
                Gizmos.matrix = Matrix4x4.identity;
                return;
            }
            Vector3 meshPos = Vector3.forward;
            meshPos.z += gizmoMesh.bounds.size.z * 0.5f;
            Gizmos.DrawWireMesh(gizmoMesh,meshPos);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
