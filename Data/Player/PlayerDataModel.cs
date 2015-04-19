using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player
{
	public abstract class PlayerDataModel
	{
		public PlayerDataModel() {}

		public abstract void Reset();
		public abstract bool Merge(PlayerDataModel data);
	}
}
