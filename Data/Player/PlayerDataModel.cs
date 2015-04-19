using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player.Models
{
	public abstract class PlayerDataModel
	{
		public PlayerDataModel() {}

		public abstract void Reset();
		public abstract bool Merge(PlayerDataModel data);
	}
}
