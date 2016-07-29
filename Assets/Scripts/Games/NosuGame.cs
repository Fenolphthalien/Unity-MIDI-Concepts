using UnityEngine;
using UnityMIDI;
using System.Collections;

namespace NOsu{
	public class NosuGame : MonoBehaviour, INoteOnHandler
	{
		[SerializeField]
		PlayAreaView playAreaView = null;

		[SerializeField]
        AnimationCurve m_pulseCurve = null;

		[SerializeField]
        NosuPlayerControl m_playerControl = null;

		float animationTime;

		public EVisualiserConfiguration visualiseAs;
		public EVisualiserDirection visualiserDirection;

		[SerializeField]
        NosuEmitter m_Cobj = null, m_CSobj = null, m_Dobj = null, m_DSobj = null, m_Eobj = null, m_Fobj = null, m_FSobj = null, m_Gobj = null, m_GSobj = null, m_Aobj = null, m_ASobj = null, m_Bobj = null;

		const int kSides = 12;
		const float kEmitterOffset = 1.5f, kStartRot = 90;
		const float kDivider = 360f / (kSides);

		float[] m_sine, m_cosine;

		public float preDelay;
		float m_elapsedDelay, m_timeElapsed, m_stageTime;

		bool m_paused = true;

		EnOsuGameState m_gameState;

		[SerializeField]
        GameObject m_mainMenu = null;

        [SerializeField]
        CanvasGroup m_instructions = null;

		[SerializeField]
		MIDIPlayer m_midiPlayer = null;

		[SerializeField]
        MIDI m_midi = null;

        int m_track = 0;

		[SerializeField]
        AudioSource m_audioSource = null;

        bool playerKilled { get { return m_gameState == EnOsuGameState.PLAY && m_playerControl.IsPlayerDead(); } }
        bool stageComplete { get { return m_gameState == EnOsuGameState.PLAY && m_timeElapsed >= m_stageTime; } }

		void Awake () 
		{
			m_sine = new float[kSides];
			m_cosine = new float[kSides];
			for(int i = 0; i < kSides; i++)
			{
				m_sine[i] = Mathf.Sin((kDivider * i + kStartRot)  * Mathf.Deg2Rad);
				m_cosine[i] = Mathf.Cos((kDivider * i + kStartRot)  * Mathf.Deg2Rad) * (int)visualiserDirection;
			}
		}

		void Start()
		{
			ChangeState (m_gameState);
			if (m_midi != null && m_midiPlayer != null)
				m_midiPlayer.SetMIDI (m_midi);
		}

		void InitialiseNewGame()
		{
			m_playerControl.EnablePlayerInput (false);
			m_playerControl.FillHealth ();
			m_midiPlayer.Stop ();
            m_midiPlayer.SetMIDI(m_midi);
            m_midiPlayer.SwitchTrack(m_track);
			ClearEmmiters ();
            m_timeElapsed = 0;
			if(playAreaView != null)
			{
				NosuEmitter[] tokenArray = BuildEmitterArray();
				float radius = playAreaView.borderRadius + kEmitterOffset;
				
				int steps = 0, i = 0;
				if(visualiseAs == EVisualiserConfiguration.A_MINOR_SCALE || visualiseAs == EVisualiserConfiguration.AEOLIAN_COF)
				{
					i = 9; // A is 9 semitones from C.
				}
				while(steps < 12) // 12 semitones in a scale
				{
					if(tokenArray[i] != null){
						tokenArray[i].Initialise(transform.position + (new Vector3(m_cosine[steps],0,m_sine[steps])*radius), UnityMIDIPreferences.GetColor((Tone)i));
					}
					if(visualiseAs == EVisualiserConfiguration.IONIAN_COF || visualiseAs == EVisualiserConfiguration.AEOLIAN_COF)
					{
						i = (i+7)%12;
					}
					else
					{
						i = (i+1)%12;
					}
					steps++;
				}
				m_paused = false;
				playAreaView.gameObject.SetActive(true);
				m_playerControl.gameObject.SetActive(true);
                SetMenuVisibility(false);
			}
			StartCoroutine (PreGameDelay ());
		}

		void StartNewGame()
		{
            const int audioDelay = 1;
			m_playerControl.EnablePlayerInput (true);
			m_midiPlayer.Play ();
			if(m_audioSource != null)
			{
				m_audioSource.clip = m_midi.audioClip;
				m_audioSource.PlayDelayed(audioDelay);
			}
            m_stageTime = m_midi.secondsRoundedUp + audioDelay;
		}

		void EndGame()
		{
			m_playerControl.EnablePlayerInput (false);
			playAreaView.gameObject.SetActive(false);
			m_playerControl.gameObject.SetActive(false);
            SetMenuVisibility(true);
			ClearEmmiters ();
			m_midiPlayer.Stop ();
		}

        public void StartNewGame(MIDI _midi, int trackIndex, float _delay = -1)
        {
            if(_midi == null)
                return;
            preDelay = _delay >= 0 ? _delay : preDelay;
            m_midi = _midi;
            m_track = trackIndex >= 0 ? trackIndex : 0; 
            ChangeState(EnOsuGameState.PRE_PLAY);
        }

		public void SetGamePause(bool b)
		{
			m_paused = b;
		}

		public void ToggleGamePause()
		{
			m_paused = !m_paused;
		}

		public bool IsGamePaused()
		{
			return m_paused;
		}

		public void ChangeState(EnOsuGameState newState)
		{
			Shader.SetGlobalVector ("_Pulse", Vector4.zero);
			m_gameState = newState;
			if(m_gameState == EnOsuGameState.PRE_PLAY)
				InitialiseNewGame ();
			else if(m_gameState == EnOsuGameState.PLAY)
				StartNewGame();
			else
				EndGame();
		}

		public void ChangeState(int i)
		{
			ChangeState ((EnOsuGameState)i);
		}

		public NosuPlayerControl GetPlayer()
		{
			return m_playerControl;
		}

		public void FireEmitter(int at)
		{
			int i = at % kSides; // Make sure input is in range.
			NosuEmitter[] emmiters = BuildEmitterArray ();
			if(emmiters != null)
			{
				emmiters[i].PrepareToFire(m_playerControl);
			}
		}

		public void ClearEmitter(int at)
		{
			int i = at % kSides; // Make sure input is in range.
			NosuEmitter[] emmiters = BuildEmitterArray ();
			if(emmiters != null)
			{
				emmiters[i].Clear();
			}
		}

		void ClearEmmiters()
		{
			NosuEmitter[] emmiters = BuildEmitterArray ();
			for (int i = 0; i < emmiters.Length; i++) 
			{
				if(emmiters[i] != null)
				{
					emmiters[i].Clear();
				}
			}
		}

		void Update()
		{
			if (playerKilled || stageComplete)
				ChangeState (EnOsuGameState.MENU);
           
            if (m_gameState == EnOsuGameState.PLAY)
                m_timeElapsed += Time.deltaTime;
		}

		void LateUpdate()
		{
			if (m_pulseCurve != null && !m_paused && m_gameState == EnOsuGameState.PLAY) 
			{
				Vector4 _pulse = new Vector4 (m_pulseCurve.Evaluate (animationTime), animationTime, 0, 0);
				Shader.SetGlobalVector ("_Pulse", _pulse);
				animationTime += Time.deltaTime;
			}
		}

		NosuEmitter[] BuildEmitterArray()
		{
			return new NosuEmitter[]{m_Cobj,m_CSobj,m_Dobj,m_DSobj,m_Eobj,m_Fobj,m_FSobj,m_Gobj,m_GSobj,m_Aobj,m_ASobj,m_Bobj};
		}
		
		public void OnNoteOn(MIDIMessage midiMessage)
		{
			if (m_gameState == EnOsuGameState.PLAY && midiMessage.keyEvent.velocity > 0) {
				FireEmitter (midiMessage.keyEvent.note);
			}
		}

		IEnumerator PreGameDelay()
		{
			yield return new WaitForSeconds (preDelay);
			ChangeState (EnOsuGameState.PLAY);
			yield break;
		}

        void SetMenuVisibility(bool visibility)
        {
            if(m_mainMenu != null)
                m_mainMenu.SetActive(visibility);
            if(m_instructions != null)
                m_instructions.alpha = visibility ? 1 : 0;
        }
	}
}