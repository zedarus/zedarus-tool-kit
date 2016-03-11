using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player
{
	[Serializable] 
	public abstract class PlayerDataModel
	{
		public PlayerDataModel() {}

		public abstract void Reset();
		public abstract bool Merge(PlayerDataModel data);
	}
}
