using UnityEngine;
using System.Collections;
using UnityMIDI;

[RequireComponent(typeof(Renderer))]
public class FlickerAndFade : MonoBehaviour, IBeatHandler
{
    MeshRenderer mr;
    float rgb = 0;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        mr.material.color = new Color(rgb, rgb, rgb, 1);
        rgb = rgb - Time.deltaTime <= 0 ? 0 : rgb - Time.deltaTime;
    }

    public void Flicker()
    {
        rgb = 1;
    }

    public void OnBeat()
    {
        Flicker();
    }
}
