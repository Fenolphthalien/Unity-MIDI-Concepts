using UnityEngine;
using System.Collections;

namespace PrototypeFour
{
	public class GameplayArgs : StateChangeEventArg
	{
		public readonly int playerHealth, totalFloorHealth;
		public readonly bool playerSuccessfull;
		
		public GameplayArgs(BaseGameStateControl sender, bool win) : base(sender)
		{
			playerSuccessfull = win;
			GameplayControl gamePlayControl = (GameplayControl)sender;
			if(gamePlayControl != null)
			{
			}
		}
	}
}
