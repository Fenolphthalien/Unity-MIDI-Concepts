using UnityEngine;
using System;
using System.Collections;
namespace PrototypeThree
{
	public class StateChangeEventArg : EventArgs
	{
		public readonly GameStateControlBase sendingObj;

		public StateChangeEventArg(GameStateControlBase sender)
		{
			sendingObj = sender;
		}
	}
}
