using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PrototypeThree
{
	public class GameOverGUIControl : MonoBehaviour 
	{
		[SerializeField]
		CanvasGroup m_guiRoot = null;

		[SerializeField]
		Text m_header = null, m_playerHealth = null, m_floorHealth = null, m_totalScore = null;

		public void SetPlayerWonValue(bool value)
		{
			if(m_header != null)
				m_header.text = value ? "SONG\nCOMPLETE" : "SONG\nFAILED";
		}

		public void SetPlayerHealthValue(int health)
		{
			if(m_playerHealth != null)
			{
				m_playerHealth.text = health.ToString();
			}
		}

		public void SetFloorHealthValue(int health)
		{
			if(m_floorHealth != null)
			{
				m_floorHealth.text = health.ToString();
			}
		}

		public void SetTotalScoreValue(int value)
		{
			if(m_totalScore != null)
			{
				m_totalScore.text = value.ToString();
			}
		}

		public void SetVisible(bool value)
		{
			if(m_guiRoot != null)
			{
				m_guiRoot.interactable = value;
				m_guiRoot.blocksRaycasts = value;
				m_guiRoot.alpha = value ? 1f : 0f;
			}
		}
	}
}
