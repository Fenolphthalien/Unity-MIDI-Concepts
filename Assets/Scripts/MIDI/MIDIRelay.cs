using UnityEngine;
using System.Collections;

namespace UnityMIDI
{
    public class MIDIRelay : MIDIDispatcher, INoteOnHandler, INoteOffHandler, IAftertouchHandler
    {
        public MIDIRelay sendTo;

        bool bSafeToDispatch { get { return sendTo != null && sendTo != this; } }

        public void OnNoteOn(MIDIMessage midiMessage)
        {
            if (bSafeToDispatch)
                sendTo.Dispatch(midiMessage);
        }

        public void OnNoteOff(MIDIMessage midiMessage)
        {
            if (bSafeToDispatch)
                sendTo.Dispatch(midiMessage);
        }

        public void OnAftertouch(MIDIMessage midiMessage)
        {
            if (bSafeToDispatch)
                sendTo.Dispatch(midiMessage);
        }
    }
}
