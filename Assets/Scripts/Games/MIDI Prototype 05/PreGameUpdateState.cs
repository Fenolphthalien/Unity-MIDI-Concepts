using UnityEngine;
using System.Collections;
using UnityMIDI;

namespace PrototypeFive
{
    public class PreGameUpdateState : BaseUpdateState
    {
        readonly SongCard view;
        readonly LeadIn leadIn;
        readonly MIDI midi;

        public PreGameUpdateState(SongCard _view, LeadIn _leadIn, MIDI _midi)
        {
            view = _view;
            leadIn = _leadIn;
            midi = _midi;
        }

        public override void Enter()
        {
            view.Initialise(midi.GetName(), (int)midi.BPM,midi.copyright);
        }

        public override void Exit() { view.ZeroAlpha(); }

        public override bool Update()
        {
            view.Refresh(leadIn.ScaledElapsed());
            return leadIn == null || leadIn.ScaledElapsed() >= 1;
        }
    }
}
