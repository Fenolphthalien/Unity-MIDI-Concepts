using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameplayGUIControl : MonoBehaviour 
{
	[SerializeField]
	Text m_playerHealth = null, m_timeElapsed = null;

	[SerializeField]
	CanvasGroup rootObject = null;
	
	public void SetHealth(int health)
	{
		m_playerHealth.text = PlayerHealthString (health);
	}

	public void SetTime(float elapsed, float total)
	{
		m_timeElapsed.text = TimeElapsedString (elapsed,total);
	}

	string PlayerHealthString(int health)
	{
		string s = health.ToString();
		int digitDifference = 3 - s.Length;
		for(int i = 0; i < digitDifference; i++)
		{
			s = "0"+s;
		}
		return string.Format ("Player Health: {0}", s);
	}

	string TimeElapsedString(float elapsedTime, float totalTime)
	{
		float elapsed = Mathf.FloorToInt (elapsedTime * 100) * 0.01f,
		total = Mathf.FloorToInt (totalTime * 100) * 0.01f;
		return string.Format ("Time: {0}/{1}", elapsed, total);
	}

	public void DisplayGUI(bool display)
	{
		if(rootObject != null)
		{
			rootObject.alpha = display ? 1 : 0;
			rootObject.interactable = display;
			rootObject.blocksRaycasts = display;
		}
	}
	
}
