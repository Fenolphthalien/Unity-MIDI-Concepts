using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Tween;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    CanvasGroup m_canvasGroup;
    float target = -1;
    public float minAlpha, maxAlpha = 1;

    void OnEnable()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        target = target < 0 ? m_canvasGroup.alpha : target;
    }

    void Update()
    {
        float f = m_canvasGroup.alpha;
        Move.MoveTo(ref f, target);
        m_canvasGroup.alpha = f;
    }

    public void FadeIn()
    {
        target = maxAlpha;
    }

    public void FadeOut()
    {
        target = minAlpha;
    }
}
