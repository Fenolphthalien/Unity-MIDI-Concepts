using UnityEngine;
using System.Collections;
using Tween;

namespace UnityMIDI
{
	public class VelocityVisualiserReciever : MonoBehaviour {


		public Tone note;
		public int Octave;

		Vector3 source, push;
		
		public Vector3 direction = -Vector3.forward;
		public int strength = 5;

		Material m;
		public bool Sticcato;

		bool bDown;

		void Awake (){
			source = transform.position;

			if (GetComponent<MeshRenderer> ()) {
				m = GetComponent<MeshRenderer> ().material;
				if(note.Black())
					m.color = Color.black;
				else
					m.color = Color.white;
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
			transform.position = source + push;
			if(!bDown){
				Move.EaseTo (ref push, Vector3.zero,2);
				if (note.Black ())
					m.color = m.color.MoveTo (Color.black);
				else
					m.color = m.color.MoveTo (Color.white);
			}
		}

		void OnNoteOn(MIDIMessage midiMessage){
			if (note == (Tone)midiMessage.keyEvent.note && Octave == midiMessage.keyEvent.octave) {
				push = direction * (strength * ((float)midiMessage.keyEvent.velocity / 127));
				if (!Sticcato)
					bDown = true;
				m.color = midiMessage.keyEvent.ToColor ();
			}
		}
		
		void OnNoteOff(MIDIMessage midiMessage){
			bDown = false;
		}
		
		void OnTrackFinished(MIDIMessage midiMessage){
			bDown = false;
		}

	}
}
