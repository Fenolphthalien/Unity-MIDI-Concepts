using UnityEngine;
using System.Collections;
using UnityMIDI;


[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class MetronomeTest : MonoBehaviour
{
    public Metronome metronome = null;
    public MeshRenderer meshRenderer;

    public Mesh debugMesh;
    public Material debugMat,mrMat;

    readonly Vector3 offset = Vector3.right * 3f;

    void Update()
    {
        if (metronome != null && meshRenderer != null)
        {
            bool onBeat = metronome.IsInBeatOffset();
            meshRenderer.material.color = onBeat ? Color.white : Color.black;
            mrMat = meshRenderer.material;
        }
    }

    void LateUpdate()
    {
        if (debugMat != null && debugMesh != null && metronome != null)
        {
            Draw(metronome.ScaleBeatElapsed());
        }
    }

    void Draw(float t)
    {
        Material mat = mrMat == null ? debugMat : mrMat;
        const uint width = 3;
        const int start = (int)width * -1 - 1, end = (int)width;

        Vector3 pos = this.transform.position + Vector3.down + offset * start;
        pos.x += offset.x * t;

        Matrix4x4 matrix;
        for (int i = start; i <= end; i++)
        {
            matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Graphics.DrawMesh(debugMesh, matrix, mat, 0);
            pos.x += offset.x;
        }
    }
}
