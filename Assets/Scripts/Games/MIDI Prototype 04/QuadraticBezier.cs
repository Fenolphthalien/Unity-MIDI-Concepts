using UnityEngine;
using System.Collections;

[System.Serializable]
public class QuadraticBezier
{
    public Vector3 p0, p2;
    public float lerp = 0.5f, height = 1;

    public Vector3 p1 { get { return Vector3.Lerp(p0, p2, lerp) + Vector3.up * height; } }

    public QuadraticBezier()
    {
        p0 = Vector3.zero; 
        p2 = Vector3.one;
    }

    public Vector3 Evaluate(float t)
    {
        float point = (t > 0 ? t : 0) <= 1f ? t : 1f; // 0 <= t <= 1
        return (1 - point) * ((1 - point) * p0 + point * p1) + point * ((1 - point) * p1 + point * p2);
    }
}

[System.Serializable]
public class QuadraticBezierTransform
{
    public Transform A, B;
    public float lerp = 0.5f, height = 1;

    public Vector3 p0 { get { return A.position; } }
    public Vector3 p1 { get { return Vector3.Lerp(p0, p2, lerp) + Vector3.up * height; } }
    public Vector3 p2 { get { return B.position; } }

    public QuadraticBezierTransform(Transform a, Transform b, float _height, float _lerp = 0.5f)
    {
        A = a;
        B = b;
        height = _height;
        lerp = _lerp;
    }

    public Vector3 Evaluate(float t)
    {
        if(A == null || B == null)
            return Vector3.zero;
        float point = (t > 0 ? t : 0) <= 1f ? t : 1f; // 0 <= t <= 1
        return (1 - point) * ((1 - point) * p0 + point * p1) + point * ((1 - point) * p1 + point * p2);
    }
}
