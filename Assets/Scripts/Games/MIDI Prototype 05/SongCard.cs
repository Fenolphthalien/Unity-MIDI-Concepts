using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SongCard : MonoBehaviour 
{
    public AnimationCurve alphaCurve;

    public CanvasGroup group;
    public Text songNameText, bpmText, copyrightText;

    void Awake()
    {
        ZeroAlpha();
    }

    public void Initialise(string songName, int bpm, string copyright)
    {
        songNameText.text = songName;
        bpmText.text = string.Format("BPM : {0}", bpm.ToString());
        copyrightText.text = copyright;
    }

    public void Refresh(float t)
    {
        group.alpha = alphaCurve.Evaluate(t);
    }

    public void ZeroAlpha()
    {
        group.alpha = 0;
    }
}
