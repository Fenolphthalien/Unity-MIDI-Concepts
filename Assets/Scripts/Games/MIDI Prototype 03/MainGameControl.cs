using UnityEngine;
using System.Collections;

namespace PrototypeThree{

	public enum EGameState {MainMenu, GamePlay, GameOver};

	public class MainGameControl : MonoBehaviour
	{
		[SerializeField]
		GameplayControl m_gamePlayControl = null;

		[SerializeField]
		MainMenuControl m_mainMenuControl = null;

		[SerializeField]
		GameOverControl m_gameOverControl = null;

		GameStateControlBase m_currentState;

		public EGameState startState;

		void Awake()
		{
			m_gamePlayControl.Initialise (this);
			m_mainMenuControl.Initialise (this);
			m_gameOverControl.Initialise (this);

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
					m_currentState = m_gameOverControl;
					break;
			}
			if (m_currentState != null)
				m_currentState.EnterState (senderArgs);
		}

		public void ChangeState(GameStateControlBase state, StateChangeEventArg senderArgs)
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
