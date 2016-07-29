using UnityEngine;
using UnityMIDI;
using System.Collections;
using Tween;

public class MIDITestReciever : MonoBehaviour {

	public bool Sticcato;

	MeshRenderer mr;
	Material m;
	bool bToBlack = true;

	// Use this for initialization
	void Start () {
		if (GetComponent<MeshRenderer> ()){
			mr = GetComponent<MeshRenderer> ();
			m = mr.material;
			m.color = Color.black;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (bToBlack) {
			m.color = m.color.MoveTo(Color.black);
		}
	}

	void OnNoteOn(MIDIMessage midiMessage){
		if(!Sticcato)
			bToBlack = false;
		if(mr)
			m.color = midiMessage.keyEvent.ToColor ();
	}

	void OnNoteOff(MIDIMessage midiMessage){
		bToBlack = true;
	}

	void OnTrackFinished(MIDIMessage midiMessage){
		bToBlack = true;
	}

}
