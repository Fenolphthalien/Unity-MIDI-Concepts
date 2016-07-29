using UnityEngine;
using System.Collections;

namespace PrototypeFive
{
    public delegate void GameStateEvent(BaseGameState state, bool callExit);

    public abstract class BaseGameState : MonoBehaviour
    {
        protected StateChangeArgs lastArgs;

        protected IApplicationMain application;

        public void Initialise(IApplicationMain main)
        { 
            application = main;
            OnExitState();
        }

        public virtual void OnEnterState(BaseGameState callingState)
        {
            lastArgs = callingState.GetArgs();
        }

        public abstract void OnReadyState();

        public virtual void OnPreExitState() { }

        public abstract void OnExitState();

        public abstract StateChangeArgs GetArgs();
    }

    public interface IApplicationMain
    {
        void ChangeStateImplicitly(GameState newState, BaseGameState sender);
        void SetTransition(bool transition);
    }

    public class StateChangeArgs
    {
        public UnityMIDI.MIDI midiFile;
        public int midiTrack;
    }
}