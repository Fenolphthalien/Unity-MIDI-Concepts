using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class SelectionViewPresentation : MonoBehaviour {

	[SerializeField]
	protected Button m_previousButton, m_nextButton;
	
	[SerializeField]
	protected Text m_text;

	protected int index = 0;

	void Awake()
	{
		OnAwake ();
		UpdateText ();
	}

	void OnEnable()
	{
		SubscribeButtonEvents ();
	}
	
	void OnDisable()
	{
		UnsubscribeButtonEvents ();
	}

	void SubscribeButtonEvents()
	{
		if(m_previousButton != null)
		{
			m_previousButton.onClick.AddListener(PreviousButtonPressed);
		}
		if(m_nextButton != null)
		{
			m_nextButton.onClick.AddListener(NextButtonPressed);
		}
	}

	 void UnsubscribeButtonEvents()
	{
		if(m_previousButton != null)
		{
			m_previousButton.onClick.RemoveListener(PreviousButtonPressed);
		}
		if(m_nextButton != null)
		{
			m_nextButton.onClick.RemoveListener(NextButtonPressed);
		}
	}

	
	protected void PrintIndex()
	{
		Debug.Log (string.Format ("{0} index is: {1}", gameObject.name, index));
	}

	protected abstract void OnAwake();

	protected abstract void UpdateText();

	protected abstract void PreviousButtonPressed();

	protected abstract void NextButtonPressed();
}
