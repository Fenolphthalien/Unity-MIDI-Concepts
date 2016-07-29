using UnityEngine;
using System;
using System.Collections;
namespace PrototypeFour
{
	public class StateChangeEventArg : EventArgs
	{
		public readonly BaseGameStateControl sendingObj;

		public StateChangeEventArg(BaseGameStateControl sender)
		{
			sendingObj = sender;
		}
	}
}
