using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class PromoReward : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Name")]
		private string _name = "Reward";
		#endregion

		#region Initalization
		public PromoReward() : base() { }
		public PromoReward(int id) : base(id) { }
		#endregion

		#region Getters
		public bool Enabled
		{
			get { return _enabled; }
		}

		public string Name
		{
			get { return _name; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return base.ListName + " " + Name; }
		}
		#endregion
		#endif
	}
}

