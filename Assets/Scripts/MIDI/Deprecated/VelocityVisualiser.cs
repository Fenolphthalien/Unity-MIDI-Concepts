using UnityEngine;
using System.Collections;
using Tween;

public class VelocityVisualiser : MonoBehaviour
{

	Vector3 source, push;

	public Vector3 direction = -Vector3.forward;
	public int strength = 5;

	bool triggered;

	void Start (){
		source = transform.position;
	}
	
	// Update is called once per frame
	void Update (){
		if (!triggered) {
			transform.position = source + push;
			Move.EaseTo (ref push, Vector3.zero, 2);
		}
	}

	public void Trigger(int v){
		push = direction * (strength * ((float)v / 127));
		transform.position = source + push;
		triggered = true;
	}

	public void Release()
	{
		triggered = false;
	}

	public void SetMaterial(Material m)
	{
		GetComponent<MeshRenderer> ().material = m;
	}
}
