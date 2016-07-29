using UnityEngine;
using System.Collections;
using UnityMIDI;

public class SendMessageOnNote : MonoBehaviour, INoteOnHandler, INoteOffHandler
{
    public Tone noteMask;
    public GameObject destObj; // Sends messages to self if left null.

    public string[] noteOnMessages, noteOffMessages;


    public void OnNoteOn(MIDIMessage midiMessage)
    {
        if (midiMessage.GetNote() != noteMask)
            return;
       
        if (noteOnMessages == null)
            return;
        Dispatch(ref noteOnMessages);
    }

    public void OnNoteOff(MIDIMessage midiMessage)
    {
        if (midiMessage.GetNote() != noteMask)
            return;
        
        if (noteOnMessages == null)
            return;
        Dispatch(ref noteOffMessages);
    }

    void Dispatch(ref string[] messages)
    {
        GameObject targetObj = destObj != null ? destObj : gameObject;
        for (int i = 0; i < messages.Length; i++)
        {
            targetObj.SendMessage(messages[i],SendMessageOptions.DontRequireReceiver);
        }
    }
}
