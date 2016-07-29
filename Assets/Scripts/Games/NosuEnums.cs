using UnityEngine;
using System.Collections;

namespace NOsu
{
	public enum EnOsuGameState
	{
		MENU = 0,
		PRE_PLAY = 1,
		PLAY = 2
	};

	public enum EVisualiserConfiguration
	{
		C_MAJOR_SCALE, 
		A_MINOR_SCALE,
		IONIAN_COF, 
		AEOLIAN_COF, 
		COUNT
	};

	public enum EVisualiserDirection
	{
		NORMAL = 1, 
		REVERSED = -1
	};

	public enum EIgnoreDamage
	{
		NONE,
		PLAYER,
		EMMITER
	};
}
