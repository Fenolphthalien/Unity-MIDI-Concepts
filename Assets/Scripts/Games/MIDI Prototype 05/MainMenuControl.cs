using UnityEngine;
using System.Collections;
using PrototypeUIGeneric;

namespace PrototypeFive
{
    public class MainMenuControl : BaseGameState
    {
        public CanvasGroup view, instructions;
        public SongSelectionViewPresentation viewPresentation;

        public void StartNewGame()
        {
            application.SetTransition(true);
            application.ChangeStateImplicitly(GameState.Gameplay,this);
        }

        public override void OnExitState()
        {
            if (view == null)
                return;
            view.alpha = 0;
            instructions.alpha = 0;
        }

        public override void OnEnterState(BaseGameState callingState)
        {
            if (view == null)
                return;
            view.alpha = 1;
            instructions.alpha = 1;
            Cursor.visible = true;
        }

        public override StateChangeArgs GetArgs()
        {
            StateChangeArgs args = new StateChangeArgs();
            args.midiFile = viewPresentation.GetCurrentMIDI();
            args.midiTrack = viewPresentation.GetCurrentTrackIndex();
            return args;
        }

        public override void OnReadyState()
        {
            view.interactable = true;
        }

        public override void OnPreExitState()
        {
            view.interactable = false;
        }
    }
}
