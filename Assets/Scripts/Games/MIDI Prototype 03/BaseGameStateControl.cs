using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeThree
{
	public abstract class GameStateControlBase : MonoBehaviour, IMIDIEventHandler
	{

		protected MainGameControl m_mainGameControl;
        private StateChangeEventArg m_lastEnterArg;

		public virtual void Initialise(MainGameControl mainGameControl)
		{
			m_mainGameControl = mainGameControl;
			ExitState ();
		}

        public virtual void EnterState(StateChangeEventArg arg)
        {
            m_lastEnterArg = arg;
        }

		public abstract void ExitState();

        public StateChangeEventArg GetLastEntryArguments()
        {
            return m_lastEnterArg;
        }
	}
}
