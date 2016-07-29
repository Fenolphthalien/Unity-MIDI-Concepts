using UnityEngine;
using System.Collections;
using UnityMIDI;
using Tween;

public class TintObjectOnNoteOn : MonoBehaviour,INoteOnHandler
{
    
    public UnityEngine.UI.Graphic target = null;

    Color c = new Color(0, 0, 0, 0), targetC = new Color(0, 0, 0, 0);

    public float alpha = 1;

    void Update()
    {
        c = c.MoveTo(targetC);
        c.a *= alpha;
        target.color = c;
    }

    public void OnNoteOn(MIDIMessage midiMessage)
    {
        //targetC = UnityMIDIPreferences.GetColor(midiMessage.GetNote());
    }
}
