using UnityEngine;
using System.Collections;

namespace UnityMIDI
{
    public class BeatEventRelay : BaseBeatEventDispatcher, IBeatHandler, IStrongBeatHandler
    {
        [SerializeField]
        BeatEventRelay m_targetRelay = null;

        bool bSafeToDispatch { get { return m_targetRelay != null && m_targetRelay != this; } }

        public void OnBeat()
        {
            if (bSafeToDispatch)
                m_targetRelay.SendOnBeat();
        }

        public void OnStrongBeat()
        {
            if (bSafeToDispatch)
                m_targetRelay.SendStrongBeat();
        }
    }
}