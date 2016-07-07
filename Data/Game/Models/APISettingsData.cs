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

		[DataGroup("Remove Ads IAP")]
		[SerializeField]
		[DataField("Remove Ads IAP ID")]
		private string _removeAdsIAPID = "";

		[SerializeField]
		[DataField("Remove Ads IAP App Store ID")]
		private string _removeAdsIAPAppleID = "";

		[SerializeField]
		[DataField("Remove Ads IAP Google Play ID")]
		private string _removeAdsIAPGooglePlayID = "";

		[SerializeField]
		[DataField("Promo Ads Enabled")]
		private bool _promoAdsEnabled = false;

		[SerializeField]
		[DataField("Promo Ads Delay (min)")]
		private int _promoAdsDelayMins = 60;

		[SerializeField]
		[DataField("Local Notifications Enabled")]
		private bool _localNotificationsEnabled = false;
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

		public string RemoveAdsIAPID
		{
			get { return _removeAdsIAPID; }
		}

		public string RemoveAdsIAPAppleID
		{
			get { return _removeAdsIAPAppleID; }
		}

		public string RemoveAdsIAPGoogleID
		{
			get { return _removeAdsIAPGooglePlayID; }
		}

		public bool PromoAdsEnabled
		{
			get { return _promoAdsEnabled; }
		}

		public int PromoAdsDelayMins
		{
			get { return _promoAdsDelayMins; }
		}

		public bool LocalNotificationsEnabled
		{
			get { return _localNotificationsEnabled; }
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

