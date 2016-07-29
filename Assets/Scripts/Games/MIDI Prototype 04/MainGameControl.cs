using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
	public enum EGameState {MainMenu, GamePlay, GameOver};

	public class MainGameControl : MonoBehaviour
	{

        [SerializeField]
        GameObject m_gamePlayControlObj = null, m_mainMenuControlObj = null;

        GameplayControl m_gamePlayControl = null;

        MainMenuControl m_mainMenuControl = null;

		//GameOverControl m_gameOverControl;

		BaseGameStateControl m_currentState;

		public EGameState startState;

		void Awake()
		{
            if (m_gamePlayControlObj == null || m_mainMenuControlObj == null)
                return;
            m_gamePlayControl = m_gamePlayControlObj.GetComponent<GameplayControl>();
            m_mainMenuControl = m_mainMenuControlObj.GetComponent<MainMenuControl>();
            if (m_gamePlayControl == null || m_mainMenuControl == null)
                return;

			m_gamePlayControl.Initialise (this);
			m_mainMenuControl.Initialise (this);
			//m_gameOverControl.Initialise (this);

			ChangeState (startState, null);
		}

		public void ChangeState(EGameState newState, StateChangeEventArg senderArgs)
		{
			if(m_currentState != null)
				m_currentState.ExitState ();
			switch(newState)
			{
				case EGameState.MainMenu:
					m_currentState = m_mainMenuControl;
					break;
				case EGameState.GamePlay:
					m_currentState = m_gamePlayControl;
					break;
				case EGameState.GameOver:
					//m_currentState = m_gameOverControl;
					break;
			}
			if (m_currentState != null)
				m_currentState.EnterState (senderArgs);
		}

		public void ChangeState(BaseGameStateControl state, StateChangeEventArg senderArgs)
		{
			if(m_currentState != null)
				m_currentState.ExitState ();
			m_currentState = state;
			if (m_currentState != null)
				m_currentState.EnterState (senderArgs);
		}

		public void ChangeState(int stateIndex)
		{
			ChangeState ((EGameState)stateIndex,null);
		}
	}
}
