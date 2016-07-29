using UnityEngine;
using System.Collections;

namespace UnityMIDI
{
	public class Instrument : ScriptableObject 
	{
		public int attack, decay, release; //MS
		public float peak, sustain, keyBias;
		public int attackDecay{ get{ return attack + decay; }}

		bool triggered;

		float t, value;
		
		public Instrument(int a, float p, int d, float s, int r, float key)
		{
			attack = a;
			peak = p;
			decay = d;
			sustain = s;
			release = r;
			keyBias = key;
		}
		
		//<summary>
		// Returns a value from the envelope based on the inputted time in Milliseconds;
		//</summary>
		public void Trigger()
		{
			t = 0;
		}

		public void Tick()
		{
			if (t <= attack){
				value = Mathf.Lerp (0, peak, t / attack);
				return;
			}
			else if(t < attackDecay){
				value = Mathf.Lerp(peak, sustain, (t - attack)/decay);
				return;
			}
			value = sustain;
			t += Time.deltaTime * 1000;
		}

		public float GetVolume()
		{
			return value;
		}

	}
}
