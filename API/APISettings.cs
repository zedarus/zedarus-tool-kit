using UnityEngine;
using System.Collections;

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
		
		#region Controls
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
