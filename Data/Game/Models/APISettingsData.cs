using UnityEngine;
using System;
using System.Collections;

namespace Zedarus.ToolKit.Data.Game
{
	[Serializable]
	public class APISettingsData : GameDataModel, IGameDataModel
	{
		#region Properties
		[SerializeField]
		[DataField("Ads Enabled")]
		private bool _adsEnabled = true;
		[SerializeField]
		[DataField("Interstitials Delay")]
		[DataValidateMin(0)]
		private int _intertitialsDelay = 3;
		#endregion

		#region Initalization
		public APISettingsData() : base() { }
		public APISettingsData(int id) : base(id) { }
		#endregion

		#region Getters
		public bool AdsEnabled
		{
			get { return _adsEnabled; }
		}

		public int IntertitialsDelay
		{
			get { return _intertitialsDelay; }
		}
		#endregion

		#if UNITY_EDITOR
		#region Editor
		public override string ListName
		{
			get { return "API Settings"; }
		}
		#endregion
		#endif
	}
}

