using UnityEngine;
using System.Collections;
using UnityMIDI;

public class ToggleVisibleOnNote : MonoBehaviour, INoteOnHandler, INoteOffHandler
{
    public CanvasGroup target;
    public Tone toneMask;

    public bool hideOnAwake;

    void Awake()
    {
        target.alpha = hideOnAwake ? 0 : 1;
    }

    public void OnNoteOn(MIDIMessage midiMessage)
    {
        if (toneMask == midiMessage.GetNote())
            target.alpha = 1;
    }

    public void OnNoteOff(MIDIMessage midiMessage)
    {
        if (toneMask == midiMessage.GetNote())
            target.alpha = 0;
    }
}
