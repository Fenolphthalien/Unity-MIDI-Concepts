using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PrototypeUIGeneric;

namespace PrototypeFour
{
	public class MainMenuControl : BaseGameStateControl
	{
		[SerializeField]
		CanvasGroup m_mainMenuRoot = null, m_instructions = null;

		[SerializeField]
        Button m_newGameButton = null;

		[SerializeField]
        SongSelectionViewPresentation m_songSelection = null;

		public override void EnterState(StateChangeEventArg arg)
		{
			if(m_mainMenuRoot != null)
			{
				DisplayGUI(true);
			}
			if (m_newGameButton != null) 
			{
				m_newGameButton.onClick.AddListener(InvokeNewGame);
			}
		}

		public override void ExitState()
		{
			if (m_mainMenuRoot != null) 
			{
				DisplayGUI(false);
			}
		}

		public void DisplayGUI(bool display)
		{
			if(m_mainMenuRoot != null)
			{
				m_mainMenuRoot.alpha = display ? 1 : 0;
				m_mainMenuRoot.interactable = display;
				m_mainMenuRoot.blocksRaycasts = display;
                m_instructions.alpha = display ? 1 : 0;
			}
		}

		void InvokeNewGame()
		{
			if(m_songSelection != null)
				m_mainGameControl.ChangeState (EGameState.GamePlay, new NewGameArgs (this,m_songSelection.GetCurrentMIDI (), m_songSelection.GetCurrentTrackIndex ()));
		}
	}
}
