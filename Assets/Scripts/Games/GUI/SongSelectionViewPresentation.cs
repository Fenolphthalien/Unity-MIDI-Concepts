using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityMIDI;

namespace PrototypeUIGeneric{
	public class SongSelectionViewPresentation : SelectionViewPresentation
	{	
		public MIDI[] m_midis;
		MIDI m_currentMidi;

        bool loadMIDIResources { get { return m_midis == null || m_midis.Length == 0; } }

		[SerializeField]
		TrackSelectionViewPresentation m_trackSelectionVP = null;

		protected override void OnAwake()
		{
			m_midis = loadMIDIResources ? Resources.LoadAll<MIDI>("MIDIs") : m_midis; // Get all the MIDIS in the resources folder.
			if(IsMIDIArrayUsable())
			{
				m_currentMidi = m_midis [0];
				m_trackSelectionVP.UpdateTrackInfo(m_currentMidi);
			}
		}

		protected override void UpdateText()
		{
			if(m_text != null)
			{
				if(m_currentMidi != null)
				{
					m_text.text = m_currentMidi.name;
					return;
				}
				m_text.text = "No Midis Found";
			}
		}

		protected override void PreviousButtonPressed ()
		{
			if(IsMIDIArrayUsable())
			{
				//Skip over unitilaised MIDIs.
				do{
					index--;
					index = index < 0 ? m_midis.Length - 1: index;
					m_currentMidi = m_midis[index];
				}while(!m_currentMidi.initialised);

				UpdateText();
				m_trackSelectionVP.UpdateTrackInfo(m_currentMidi);
			}
		}
		
		protected override void NextButtonPressed ()
		{
			if(IsMIDIArrayUsable())
			{
				//Skip over unitilaised MIDIs.
				do{
					index = (index + 1) % m_midis.Length;
					m_currentMidi = m_midis[index];
				}while(!m_currentMidi.initialised);
				
				UpdateText();
				m_trackSelectionVP.UpdateTrackInfo(m_currentMidi);
			}
		}

		bool IsMIDIArrayUsable()
		{
			return m_midis != null && m_midis.Length > 0;
		}

		public MIDI GetCurrentMIDI()
		{
			return m_currentMidi;
		}

		public int GetCurrentTrackIndex()
		{
			return m_trackSelectionVP != null ? m_trackSelectionVP.GetTrack() : 0;
		}
	}
}