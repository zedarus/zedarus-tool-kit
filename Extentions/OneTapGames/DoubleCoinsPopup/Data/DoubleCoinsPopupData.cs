using UnityEngine;
using System;
using System.Collections;
using Zedarus.ToolKit.Data.Game;

namespace Zedarus.ToolKit.Extentions.OneTapGames.DoubleCoinsPopup
{
	[Serializable]
	public class DoubleCoinsPopupData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Min Coins")]
		private int _minCoins = 10;

		[SerializeField]
		[DataField("Sessions Delay")]
		private int _sessionsDelay = 1;

		[SerializeField]
		[DataField("Sessions Offset")]
		private int _sessionsOffset = 0;
		#endregion

		#region Initalization
		public DoubleCoinsPopupData() : base() { }
		public DoubleCoinsPopupData(int id) : base(id) { }
		#endregion

		#region Getters
		public int MinCoins
		{
			get { return _minCoins; }
		}

		public int Delay
		{
			get { return _sessionsDelay; }
		}

		public int Offset
		{
			get { return _sessionsOffset; }
		}

		public bool Enabled
		{
			get { return _enabled; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return "Double Coins Popup Settings"; }
		}
		#endregion
		#endif
	}
}

