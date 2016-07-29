using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace PrototypeFour
{
	public abstract class BaseGameStateControl : MonoBehaviour
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
