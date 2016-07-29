using UnityEngine;
using UnityMIDI;
using System.Collections;

public class TrackSelectionViewPresentation : SelectionViewPresentation
{
	int m_tracks;
	int m_format;

	bool isFormatOne {get{ return m_format == 1; }} //Check format so tempo map is not returned by accident;

	protected override void OnAwake ()
	{
	}

	protected override void UpdateText ()
	{
		if(m_text != null)
		{
			m_text.text = index.ToString();
		}
	}

	protected override void PreviousButtonPressed ()
	{
		if (m_tracks > 0) {
			index--;
			int comparison = isFormatOne ? 1 : 0;
			index = index < comparison ? m_tracks - 1 : index;
			UpdateText ();
		}
	}

	protected override void NextButtonPressed ()
	{
		if (m_tracks > 0) {
			index = (index + 1) % m_tracks;
			index = index == 0 && isFormatOne ? 1 : index;
			UpdateText ();
		}
	}

	public int GetTrack()
	{
		return index;
	}

	public void UpdateTrackInfo(MIDI midi)
	{
		m_format = midi.GetFormat ();
		m_tracks = midi.GetNumberOfTracksShort ();
		index = isFormatOne ? 1 : 0;
		UpdateText ();
	}
}
