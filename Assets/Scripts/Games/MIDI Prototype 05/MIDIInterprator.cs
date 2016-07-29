using UnityEngine;
using System.Collections;
using UnityMIDI;

namespace PrototypeFive
{
    [System.Serializable]
    public class InterpratorEvent : UnityEngine.Events.UnityEvent{}

    public class MIDIInterprator : MonoBehaviour, INoteOnHandler
    {
        const Tone enablePlayTone = Tone.C,
            fireLeftPigeon = Tone.Db,
            popupLeft = Tone.D,
            popupCenter = Tone.Eb,
            popupRight = Tone.E,
            fireRightPigeon = Tone.F,
            stopPlayTone = Tone.Gb,
            exitStageTone = Tone.G;

        public InterpratorEvent OnBeginPlay, OnFireLeftPigeon, OnPopupLeft, OnPopupCenter, OnPopupRight,OnFireRightPigeon, OnStopPlay, OnExitStage;

        public bool interprateEvents;
 
        public void OnNoteOn(MIDIMessage midiMessage)
        {

            switch (midiMessage.GetNote())
            {
                case enablePlayTone:
                    InvokeEvent(OnBeginPlay, true);
                    return;
                case fireLeftPigeon:
                    InvokeEvent(OnFireLeftPigeon);
                    return;
                case popupLeft:
                    InvokeEvent(OnPopupLeft);
                    return;
                case popupCenter:
                    InvokeEvent(OnPopupCenter);
                    return;
                case popupRight:
                    InvokeEvent(OnPopupRight);
                    return;
                case fireRightPigeon:
                    InvokeEvent(OnFireRightPigeon);
                    return;
                case stopPlayTone:
                    InvokeEvent(OnStopPlay,true);
                    return;
                case exitStageTone:
                    InvokeEvent(OnExitStage,true);
                    return;
            }
        }

        public void InvokeEvent(InterpratorEvent evnt, bool forceDispatch = false)
        {
            if (!forceDispatch && !interprateEvents)
                return;
            if (evnt != null)
                evnt.Invoke();
        }

        void OnDestroy()
        {
            ClearEvents();
        }

        void ClearEvents()
        {
            OnBeginPlay.RemoveAllListeners();
            OnBeginPlay = null;
            OnFireLeftPigeon.RemoveAllListeners();
            OnFireLeftPigeon = null;
            OnPopupLeft.RemoveAllListeners();
            OnPopupLeft = null;
            OnPopupCenter.RemoveAllListeners();
            OnPopupCenter = null;
            OnPopupRight.RemoveAllListeners();
            OnPopupRight = null;
            OnFireRightPigeon.RemoveAllListeners();
            OnFireRightPigeon = null;
            OnStopPlay.RemoveAllListeners();
            OnStopPlay = null;
            OnExitStage.RemoveAllListeners();
            OnExitStage = null;
        }
    }
}
