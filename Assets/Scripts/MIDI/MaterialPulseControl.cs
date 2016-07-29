using UnityEngine;
using System.Collections;

namespace UnityMIDI
{
	public class MaterialPulseControl : MonoBehaviour, IBeatHandler, IStrongBeatHandler
	{
		//There can only be one per scene.
		static MaterialPulseControl _singleton;
		public bool autoPlayOnStart;

		float m_elapsedTime, m_value;

		[SerializeField]
		AnimationCurve m_animCurve = null, m_strongAnimCurve = null;

		bool m_StrongBeat, m_playing;

		void Awake()
		{
			if(_singleton == null)
			{
				_singleton = this;
				return;
			}
		}

		void Start()
		{
			if(autoPlayOnStart)
			{
				PlayInstance();
			}
		}
		
		void LateUpdate () 
		{
			//Update shader value.
			if(m_playing)
			{
				m_elapsedTime += Time.deltaTime;

				if(m_StrongBeat && m_strongAnimCurve != null)
				{
					m_value = m_strongAnimCurve.Evaluate(m_elapsedTime);
				}
				else if(m_animCurve != null)
				{
					m_value = m_animCurve.Evaluate(m_elapsedTime);
				}

				Vector4 _pulse = new Vector4 (m_value,m_elapsedTime, 0, 0);
				Shader.SetGlobalVector ("_Pulse", _pulse);
			}
		}

		public void OnBeat()
		{
			//Debug.Log("Beat recieved");
			ResetInstance ();
			m_StrongBeat = true;
		}

		public void OnStrongBeat()
		{
			//Debug.Log("Strong recieved");
			ResetInstance ();
			m_StrongBeat = true;
		}

		public static void Play()
		{
			if (_singleton != null)
				_singleton.PlayInstance ();
		}

		public static void Stop()
		{
			if (_singleton != null)
				_singleton.StopInstance ();
		}

		public static void Reset()
		{
			if (_singleton != null)
				_singleton.ResetInstance ();
		}

		public void PlayInstance()
		{
			m_playing = true;
		}

		public void StopInstance()
		{
			m_playing = false;
		}

		public void ResetInstance()
		{
			m_elapsedTime = 0;
			if (this == _singleton) {
				Vector4 _pulse = new Vector4 (m_value, m_elapsedTime, 0, 0);
				Shader.SetGlobalVector ("_Pulse", _pulse);
			}
		}

		public float GetCurrentValue()
		{
			return m_value;
		}
	}
}
