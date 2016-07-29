using UnityEngine;
using System.Collections;
using UnityMIDI;

namespace PrototypeThree
{
	public class GameOverControl : GameStateControlBase
	{
		[SerializeField]
		GameOverGUIControl m_gameOverGuiControl = null;

		const int kCompletionBonus = 10000;

		[SerializeField]
		UnityEngine.UI.Button menuButton = null, retryButton = null; 

		GameplayControl m_lastGameplaySession;

        MIDI lastMIDI;
        int lastTrack;

		public override void EnterState(StateChangeEventArg arg)
		{
			GameplayResultArgs gameplayArg = (GameplayResultArgs)arg;
			m_lastGameplaySession = null;

			if(gameplayArg != null)
			{
                lastMIDI = gameplayArg.midi;
                lastTrack = gameplayArg.track;
				if(m_gameOverGuiControl)
				{
					m_gameOverGuiControl.SetPlayerWonValue(gameplayArg.playerSuccessfull);
					m_gameOverGuiControl.SetPlayerHealthValue(gameplayArg.playerHealth*100);
					m_gameOverGuiControl.SetFloorHealthValue(gameplayArg.totalFloorHealth);
					m_gameOverGuiControl.SetTotalScoreValue(gameplayArg.playerHealth*100 + gameplayArg.totalFloorHealth + (gameplayArg.playerSuccessfull ? kCompletionBonus : 0));

					if(menuButton != null)
					{
						m_lastGameplaySession = arg.sendingObj as GameplayControl;
                        if(m_lastGameplaySession)
							retryButton.onClick.AddListener(InvokeRetry);
						menuButton.interactable = m_lastGameplaySession != null; //Cannot restart if no GameplayControl is available.
					}
				}
			}
			menuButton.onClick.AddListener (InvokeMainMenu);

			DisplayGUI (true);
		}

		public override void ExitState()
		{
			DisplayGUI (false);
		}

		public override void Initialise (MainGameControl mainGameControl)
		{
			DisplayGUI (false);
			base.Initialise (mainGameControl);
		}

		void InvokeRetry()
		{
            m_mainGameControl.ChangeState(EGameState.GamePlay, new NewGameArgs(this,lastMIDI,lastTrack));
		}

		void InvokeMainMenu()
		{
			m_mainGameControl.ChangeState (EGameState.MainMenu, null);
		}

		void DisplayGUI(bool display)
		{
			if (m_gameOverGuiControl != null)
				m_gameOverGuiControl.SetVisible (display);
		}
	}
}
