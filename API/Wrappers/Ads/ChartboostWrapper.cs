using UnityEngine;
using System;
using System.Collections;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	#if API_CHARTBOOST_P31
	public class ChartboostWrapper : APIWrapper<ChartboostWrapper>, IInterstitialsAdsWrapperInterface
	{
		#region Events
		public event Action InterstitialClosed;
		#endregion

		#region Setup
		protected override void Setup()
		{
			Chartboost.init(APIManager.Instance.Settings.ChartboostID, APIManager.Instance.Settings.ChartboostSecret, APIManager.Instance.Settings.ChartboostID, APIManager.Instance.Settings.ChartboostSecret, true);
		}
		#endregion

		#region Controls
		public void CacheBootupAd() 
		{
			Chartboost.cacheInterstitial(APIManager.Instance.Settings.ChartboostLocationStartup);
		}

		public void CacheBetweenLevelAd()
		{
			Chartboost.cacheInterstitial(APIManager.Instance.Settings.ChartboostLocationLevelComplete);
		}

		public void CacheMoreGamesAd()
		{
			Chartboost.cacheMoreApps();
		}

		public void ShowBootupAd()
		{
			#if UNITY_EDITOR
			if (InterstitialClosed != null)
				InterstitialClosed();
			#else
			Chartboost.showInterstitial(APIManager.Instance.Settings.ChartboostLocationStartup);
			#endif
		}

		public void ShowBetweenLevelAd()
		{
			#if UNITY_EDITOR
			if (InterstitialClosed != null)
				InterstitialClosed();
			#else
			Chartboost.showInterstitial(APIManager.Instance.Settings.ChartboostLocationLevelComplete);
			#endif
		}

		public void ShowMoreGamesAd()
		{
			Chartboost.showMoreApps();
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			Chartboost.didCacheInterstitialEvent += didCacheInterstitialEvent;
			Chartboost.didFailToCacheInterstitialEvent += didFailToLoadInterstitialEvent;
			Chartboost.didFinishInterstitialEvent += didFinishInterstitialEvent;
			Chartboost.didCacheMoreAppsEvent += didCacheMoreAppsEvent;
			Chartboost.didFailToCacheMoreAppsEvent += didFailToCacheMoreAppsEvent;
			Chartboost.didFinishMoreAppsEvent += didFinishMoreAppsEvent;
		}
		
		protected override void RemoveEventListeners()
		{
			Chartboost.didCacheInterstitialEvent -= didCacheInterstitialEvent;
			Chartboost.didFailToCacheInterstitialEvent -= didFailToLoadInterstitialEvent;
			Chartboost.didFinishInterstitialEvent -= didFinishInterstitialEvent;
			Chartboost.didCacheMoreAppsEvent -= didCacheMoreAppsEvent;
			Chartboost.didFailToCacheMoreAppsEvent -= didFailToCacheMoreAppsEvent;
			Chartboost.didFinishMoreAppsEvent -= didFinishMoreAppsEvent;
		}
		#endregion

		#region Event Handlers
		private void didFailToCacheMoreAppsEvent(string location, string error)
		{
			ZedLogger.Log("didFailToCacheMoreAppsEvent: " + location + ", error: " + error);
		}

		private void didCacheInterstitialEvent(string location)
		{
			ZedLogger.Log("didCacheInterstitialEvent: " + location);
		}

		private void didCacheMoreAppsEvent(string location)
		{
			ZedLogger.Log("didCacheMoreAppsEvent: " + location);
		}

		private void didFinishInterstitialEvent(string location, string reason)
		{
			ZedLogger.Log("didFinishInterstitialEvent: " + location + ", reason: " + reason);
			if (InterstitialClosed != null)
				InterstitialClosed();
		}

		private void didFinishMoreAppsEvent(string location, string reason)
		{
			ZedLogger.Log("didFinishMoreAppsEvent: " + location + ", reason: " + reason);
		}

		private void didFailToLoadInterstitialEvent(string location, string error)
		{
			ZedLogger.Log("didFailToLoadInterstitialEvent: " + location + ", error: " + error);
			if (InterstitialClosed != null)
				InterstitialClosed();
		}
		#endregion
	}
	#endif
}
