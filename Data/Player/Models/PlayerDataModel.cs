using UnityEngine;
using System.Collections;

namespace Zedarus.ToolKit.Data.Player.Models
{
	public class PlayerDataModel : IPlayerDataModel<PlayerDataModel>
	{
		public PlayerDataModel() {}

		public virtual void Reset() {}
		public bool Merge(PlayerDataModel data) {}

		public static int GetModelID()
		{
			return 0;
		}
	}
}
