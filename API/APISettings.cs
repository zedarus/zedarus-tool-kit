using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zedarus.ToolKit.API
{
	public class APISettings
	{
		#region Settings
		private string _twitterKey;
		private string _twitterSecret;
		private string _flurryKey;
		private string _icloudFilename;
		private string _chartboostID;
		private string _chartboostSecret;
		private string _chartboostLocationStartup;
		private string _chartboostLocationLevelComplete;
		private string _adMobUnitID;
		private string _adMobPublisherID;
		#endregion

		#region Properties
		private bool _iapEnabled;
		private bool _adsEnabled;
		private int _interstitialsTriggerEventsInterval;
		private Dictionary<int, string> _storeItemsKeys = new Dictionary<int, string>();
		#endregion

		#region Init
		public APISettings()
		{
			_iapEnabled = false;
			_adsEnabled = false;
			_interstitialsTriggerEventsInterval = 0;
			_storeItemsKeys = new Dictionary<int, string>();
		}
		#endregion
		
		#region Controls
		public void SetIAPEnabled(bool enabled)
		{
			_iapEnabled = enabled;
		}

		public void SetAdsEnabled(bool adsEnabled)
		{
			_adsEnabled = adsEnabled;
		}

		public void SetInterstitialsTriggerInterval(int interval)
		{
			_interstitialsTriggerEventsInterval = interval;
		}
		
		public void RegisterStoreItem(int id, string key)
		{
			_storeItemsKeys.Add(id, key);
		}

		public void SetTwitter(string key, string secret)
		{
			_twitterKey = key;
			_twitterSecret = secret;
		}
		
		public void SetFlurryKey(string key)
		{
			_flurryKey = key;
		}

		public void SetiCloudFilename(string filename)
		{
			_icloudFilename = filename;
		}

		public void SetChartboost(string id, string secret, string locationStartup, string locationLevelComplete)
		{
			_chartboostID = id;
			_chartboostSecret = secret;
			_chartboostLocationStartup = locationStartup;
			_chartboostLocationLevelComplete = locationLevelComplete;
		}

		public void SetAdMob(string unitID, string publisherID)
		{
			_adMobUnitID = unitID;
			_adMobPublisherID = publisherID;
		}
		#endregion
		
		#region Getters
		public bool AdsEnabled
		{
			get { return _adsEnabled; }
		}

		public bool IAPEnabled
		{
			get { return _iapEnabled; }
		}

		public int InterstitialsTriggerEventsInterval
		{
			get { return _interstitialsTriggerEventsInterval; }
		}

		public Dictionary<int, string> StoreItems
		{
			get { return _storeItemsKeys; }
		}

		public string TwitterKey
		{
			get { return _twitterKey; } 
		}
		
		public string TwitterSecret
		{
			get { return _twitterSecret; } 
		}
		
		public string FlurryKey
		{
			get { return _flurryKey; }
		}

		public string iCloudFilename
		{
			get { return _icloudFilename; }
		}

		public string ChartboostID
		{
			get { return _chartboostID; }
		}

		public string ChartboostSecret
		{
			get { return _chartboostSecret; }
		}

		public string ChartboostLocationStartup
		{
			get { return _chartboostLocationStartup; }
		}

		public string ChartboostLocationLevelComplete
		{
			get { return _chartboostLocationLevelComplete; }
		}

		public string AdMobPublisherID
		{
			get { return _adMobPublisherID; }
		}

		public string AdMobUnitID
		{
			get { return _adMobUnitID; }
		}
		#endregion
	}
}
