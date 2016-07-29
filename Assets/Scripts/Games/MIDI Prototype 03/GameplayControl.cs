using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityMIDI;

namespace PrototypeThree
{
	public sealed class GameplayControl : GameStateControlBase, INoteOnHandler, INoteOffHandler
	{
		[SerializeField]
		MIDIPlayer m_midiPlayer = null;

		[SerializeField, Header("Painters")]
        BlockPainter m_CPainter = null;
        
        [SerializeField]
        BlockPainter m_CSPainter = null, m_DPainter = null, m_DSPainter = null, m_EPainter = null, m_FPainter = null, m_FSPainter = null, m_GPainter = null, m_GSPainter = null, m_APainter = null, m_ASPainter = null, m_BPainter = null;

		[SerializeField, Header("References")]
		Material m_blockMaterial = null;

		[SerializeField]
		FloorControl m_floorControl = null;

		[SerializeField]
		BlockControl m_blockControl = null;

		[SerializeField]
		GameplayGUIControl m_gameplayGUIControl = null;

        [SerializeField]
        float m_floorDepth = 0;

		[SerializeField]
		PlayerControl m_player = null;

		float timeElapsed = 0, totalTime;

		const float kDelay = 2f, kMinBlockLength = 0.5f;

		public bool autoStartGame;

		bool m_gameStarted = false;

        NewGameArgs lastArg;

        MIDI midi;
        int track;

		public float floorHeight 
        {
			get 
            {
				return transform.position.y - m_floorDepth;
			}
		}

		public override void EnterState(StateChangeEventArg arg)
		{
			this.gameObject.SetActive (true);

            m_midiPlayer.Stop();
			m_midiPlayer.Reset ();

			NewGameArgs newGameArg = (NewGameArgs)arg;
            if (newGameArg == null)
                return;

            midi = newGameArg.midi;

			BlockPainter[] painters = BuildBlockPainterArray ();
			for(int i  = 0; i < painters.Length; i++)
			{
				if(painters[i] != null)
                {
					painters[i].gameObject.SetActive(true);
					painters[i].Initialise(kDelay);
				}
			}

			m_gameplayGUIControl.DisplayGUI (true);

			if (m_player != null)
				m_player.Initialise (m_floorControl);

			if (m_blockControl != null)
				m_blockControl.Initialise (floorHeight, m_floorControl);

			if (m_floorControl != null)
				m_floorControl.Initialise ();

            track = newGameArg.track;
            MIDITrack midiTrack = midi.GetTrack(track);
            totalTime = midiTrack.secondsRoundedUp;


			m_gameStarted = false;
			timeElapsed = 0;

			m_gameplayGUIControl.SetHealth (m_player.GetHealth ());
            m_midiPlayer.PlayAfter(kDelay, newGameArg.track, midi);
		}

		public override void ExitState()
		{
			BlockPainter[] painters = BuildBlockPainterArray ();
			for(int i  = 0; i < painters.Length; i++)
			{
				if(painters[i] != null)
                {
					painters[i].gameObject.SetActive(false);
				}
			}
			m_gameplayGUIControl.DisplayGUI (false);
			m_blockControl.Clear ();
			m_player.Clear ();
			this.gameObject.SetActive (false);
		}

		void Update()
		{
			if (m_midiPlayer.playing)
				m_gameStarted = true;
			if (m_gameStarted)
				timeElapsed += Time.deltaTime;

			if(m_player.damageTaken)
			{
				m_gameplayGUIControl.SetHealth (m_player.GetHealth ());
				if(m_player.GetHealth() <= 0)
				{
					m_mainGameControl.ChangeState(EGameState.GameOver,new GameplayResultArgs(this,false,midi,track));
				}
			}
			if(timeElapsed >= totalTime && m_midiPlayer.playing)
			{
				m_midiPlayer.Stop();
			}
           
            //State change condition - Success - Song has finished and no more blocks are in play.
			if(timeElapsed >= totalTime && m_blockControl.count <= 0)
			{
				m_mainGameControl.ChangeState(EGameState.GameOver,new GameplayResultArgs(this,true,midi,track));
			}

			m_gameplayGUIControl.SetTime (timeElapsed,totalTime);
		}

		void SpawnNewMidiBlock(BlockPainter blockPainter)
		{
			if (blockPainter != null && blockPainter.triggered)
			{
                float width, height, xPositive, xNegative;
                Vector3 position;

				Mesh mesh = blockPainter.Release(out width, out height,out xNegative, out xPositive, out position);
                
                //Return if block width is too short.
                if (width < kMinBlockLength)
                {
                    mesh = null;
                    return;
                }

				GameObject gameObject = new GameObject ();
				Block block = gameObject.AddComponent<Block> ();
				block.Initialise (mesh, m_blockMaterial,width,height, xNegative, xPositive,position,blockPainter.hitSound,blockPainter.hitSoundNearDeath);
				m_blockControl.Add(block);
			}
		}

		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube (transform.position + Vector3.down * m_floorDepth,Vector3.one);
		}

		BlockPainter[] BuildBlockPainterArray()
		{
			return new BlockPainter[] 
			{
				m_CPainter,
				m_CSPainter,
				m_DPainter,
				m_DSPainter,
				m_EPainter,
				m_FPainter,
				m_FSPainter,
				m_GPainter,
				m_GSPainter,
				m_APainter,
				m_ASPainter,
				m_BPainter
			};
		}

		public int GetPlayerHealth()
		{
			return m_player.GetHealth ();
		}

		public int GetFloorTotalHealth()
		{
			return m_floorControl.GetTotalHealth ();
		}

		public void OnNoteOn(MIDIMessage message)
		{
			if(message.keyEvent.velocity != 0)
			{
				BlockPainter[] blockPainter = BuildBlockPainterArray();
                if (blockPainter[message.keyEvent.note] != null)
                {
                    SpawnNewMidiBlock(blockPainter[message.keyEvent.note]);
                    blockPainter[message.keyEvent.note].Trigger(m_blockMaterial, UnityMIDIPreferences.GetColor(message.keyEvent.note));
                }
			}

		}

		public void OnNoteOff(MIDIMessage message)
		{
			BlockPainter[] blockPainter = BuildBlockPainterArray();
			if(blockPainter[message.keyEvent.note] != null)
				SpawnNewMidiBlock (blockPainter [message.keyEvent.note]);
		}
	}
}
