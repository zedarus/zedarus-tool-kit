using UnityEngine;
using System;
using System.Collections;
using Zedarus.ToolKit.Data.Game;

namespace Zedarus.ToolKit.Extentions.OneTapGames.SecondChancePopup
{
	[Serializable]
	public class SecondChancePopupData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Enabled")]
		private bool _enabled = true;

		[SerializeField]
		[DataField("Price")]
		private int _price = 20;

		[SerializeField]
		[DataField("Display Every X Session")]
		private int _delay = 1;

		[SerializeField]
		[DataField("Initial Sessions Offset")]
		private int _offset = 0;

		[SerializeField]
		[DataField("Mandatory Display Score")]
		private int _scoreGap = 10;
		#endregion

		#region Initalization
		public SecondChancePopupData() : base() { }
		public SecondChancePopupData(int id) : base(id) { }
		#endregion

		#region Getters
		public bool Enabled
		{
			get { return _enabled; }
		}

		public int Price
		{
			get { return _price; }
		}

		public int Delay
		{
			get { return _delay; }
		}

		public int Offset
		{
			get { return _offset; }
		}

		public int MandatoryScore
		{
			get { return _scoreGap; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return "Second Chance Popup Settings"; }
		}
		#endregion
		#endif
	}
}

