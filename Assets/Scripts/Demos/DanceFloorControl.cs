using UnityEngine;
using System.Collections;
using UnityMIDI;

public class DanceFloorControl : MonoBehaviour, INoteOnHandler, INoteOffHandler
{
    Texture2D m_globalMIDIMaskReference, m_globalOctaveMaskReference;

    void OnEnable()
    {
        m_globalMIDIMaskReference = MIDITextureMasks.GenerateFullMIDIMask(true);
        m_globalOctaveMaskReference = MIDITextureMasks.GenerateOctaveMask(true);
        Debug.Log(m_globalOctaveMaskReference);
    }

    void OnDisable()
    {
        MIDITextureMasks.NullMIDITexture(m_globalMIDIMaskReference);
        MIDITextureMasks.NullOctaveTexture(m_globalOctaveMaskReference);
    }

    public void OnNoteOn(MIDIMessage message) 
    {
        if (message.IsNoteOff())
        {
            OnNoteOff(message);
            return;
        }
       int x, y;
       MIDITextureMasks.GetMIDIMaskPixel(message.keyEvent,out x, out y);
       m_globalMIDIMaskReference.SetPixel(x, y, Color.white);
       m_globalMIDIMaskReference.Apply();

       Debug.Log(string.Format("x:{0},y:{1}", x, y));

       MIDITextureMasks.GetOctaveMaskPixel(message.keyEvent, out x, out y);
       m_globalOctaveMaskReference.SetPixel(x, y, Color.white);
       m_globalOctaveMaskReference.Apply();
    }

    public void OnNoteOff(MIDIMessage message)
    {
        int x, y;
        MIDITextureMasks.GetMIDIMaskPixel(message.keyEvent, out x, out y);
        m_globalMIDIMaskReference.SetPixel(x, y, Color.black);
        m_globalMIDIMaskReference.Apply();

        MIDITextureMasks.GetOctaveMaskPixel(message.keyEvent, out x, out y);
        m_globalOctaveMaskReference.SetPixel(x, y, Color.black);
        m_globalOctaveMaskReference.Apply();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
            MIDITextureMasks.LogMIDITexture(null, m_globalMIDIMaskReference);
        else if (Input.GetKeyDown(KeyCode.Home))
            MIDITextureMasks.LogOctaveTexture(null, m_globalOctaveMaskReference);
    }
}
