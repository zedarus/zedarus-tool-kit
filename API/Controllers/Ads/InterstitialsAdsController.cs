using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Settings;
using Zedarus.Traffico.Data.PlayerData;

namespace Zedarus.ToolKit.API
{
	public class InterstitialsAdsController : APIController 
	{	
		#region Events
		public event Action InterstitialClosed;
		#endregion

		#region Initialization
		public InterstitialsAdsController(MultipleAPIUseMode useMode, params APIs[] values) : base(useMode, values) {}
		protected override void Setup() {}	
		#endregion
		
		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				#if API_CHARTBOOST_P31
				case APIs.Chartboost:
					return ChartboostWrapper.Instance;
				#endif
				default:
					return null;
			}
		}
		#endregion
		
		#region Controls
		public void CacheBootupAd()
		{
			if (!Enabled)
				return;

			foreach (IInterstitialsAdsWrapperInterface wrapper in _wrappers)
				wrapper.CacheBootupAd();
		}

		public void CacheBetweenLevelAd()
		{
			if (!Enabled)
				return;
			
			foreach (IInterstitialsAdsWrapperInterface wrapper in _wrappers)
				wrapper.CacheBetweenLevelAd();
		}

		public void CacheMoreGamesAd()
		{
			if (!Enabled)
				return;
			
			foreach (IInterstitialsAdsWrapperInterface wrapper in _wrappers)
				wrapper.CacheMoreGamesAd();
		}

		public void ShowBootupAd()
		{
			IInterstitialsAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.ShowBootupAd();
		}

		public void ShowBetweenLevelAd()
		{
			bool canShowAd = ShouldDisplayBetweenLevelAd();
			IInterstitialsAdsWrapperInterface wrapper = Wrapper;
			
			if (canShowAd)
				PlayerDataManager.Instance.ResetIntersititalsEvents();

			if (Enabled && wrapper != null && canShowAd)
				wrapper.ShowBetweenLevelAd();
		}

		public void ShowMoreGamesAd()
		{
			IInterstitialsAdsWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
				wrapper.ShowMoreGamesAd();
		}
		#endregion

		#region Queries
		public bool ShouldDisplayBetweenLevelAd()
		{
			if (Enabled)
				return PlayerDataManager.Instance.GetInterstitialsEvents() >= GlobalSettings.Instance.InterstitialsLevelsInterval;
			else
				return false;
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();
			
			foreach (IInterstitialsAdsWrapperInterface wrapper in Wrappers)
			{
				wrapper.InterstitialClosed += OnInterstitialClosed;
			}
		}
		
		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();
			
			foreach (IInterstitialsAdsWrapperInterface wrapper in Wrappers)
			{
				wrapper.InterstitialClosed -= OnInterstitialClosed;
			}
		}
		#endregion
		
		#region Event Handlers
		private void OnInterstitialClosed()
		{
			if (InterstitialClosed != null)
				InterstitialClosed();
		}
		#endregion
		
		#region Getters
		protected IInterstitialsAdsWrapperInterface Wrapper
		{
			get { return (IInterstitialsAdsWrapperInterface)CurrentWrapperBase; }
		}
		#endregion

		#region Helpers
		private bool Enabled
		{
			get 
			{
				return GlobalSettings.Instance.AdsEnabled && PlayerDataManager.Instance.AdsEnabled;
			}
		}
		#endregion
	}
}
