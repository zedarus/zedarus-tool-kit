using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class MediationAdsController : APIController 
	{	
		#region Events
		public event Action InterstitialClosed;
		public event Action GrantReward;
		public event Action BannerDisplayed;
		public event Action BannerRemoved;
		#endregion

		#region Properties
		private bool _cached = false;
		#endregion

		#region Initialization
		protected override void Setup() { }
		#endregion

		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(int wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.Ads.HeyZap:
					return HeyZapWrapper.Instance;
				default:
					return null;
			}
		}
		#endregion

		#region Controls - Caching
		public void Cache()
		{
			if (_cached)
				return;

			// TODO: cache here

			_cached = true;
		}

		public void CacheInterstitial(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CacheInterstitial(tag);
		}

		public void CacheRewardedVideo(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CacheRewardedVideo(tag);
		}
		#endregion

		#region Controls
		public void ShowBanner(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.ShowBanner(tag);
		}

		public void HideBanner()
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
				wrapper.HideBanner();
		}

		public void ShowIntersitital(string tag, bool useBetweenLevelCounter)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
			{
				if (useBetweenLevelCounter)
				{
					if (ShouldDisplayBetweenLevelAd())
					{
						Debug.Log("Display interstitial: " + tag);
						APIManager.Instance.State.ResetInterstitialCounter();
						EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
						#if UNITY_EDITOR
						OnInterstitialClosed();
						#else
						wrapper.ShowIntersitital(tag);
						#endif
					}
				}
				else
				{
					EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
					#if UNITY_EDITOR
					OnInterstitialClosed();
					#else
					wrapper.ShowIntersitital(tag);
					#endif
				}
			}
		}

		public void ShowRewardedVideo(string tag, bool useBetweenLevelCounter)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
			{
				if (useBetweenLevelCounter)
				{
					if (ShouldDisplayBetweenLevelAd())
					{
						EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
						wrapper.ShowRewardedVideo(tag);
						#if UNITY_EDITOR
						OnInterstitialClosed();
						#else
						wrapper.ShowRewardedVideo(tag);
						#endif
					}
				}
				else
				{
					EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
					#if UNITY_EDITOR
					OnInterstitialClosed();
					#else
					wrapper.ShowRewardedVideo(tag);
					#endif
				}
			}
		}

		private void DisableAds()
		{
			HideBanner();
		}
		#endregion

		#region Queries
		public float GetBannerHeight()
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				return wrapper.GetBannerHeight();
			else
				return 0f;
		}

		public bool IsBannerVisible()
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				return wrapper.IsBannerVisible();
			else
				return false;
		}

		private bool ShouldDisplayBetweenLevelAd()
		{
			if (Enabled)
				return APIManager.Instance.State.IntertitialCounter >= APIManager.Instance.Settings.IntertitialsDelay;
			else
				return false;
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

			EventManager.AddListener(IDs.Events.DisableAds, OnDisableAds);

			foreach (IMediationAdsWrapperInterface wrapper in Wrappers)
			{
				wrapper.InterstitialClosed += OnInterstitialClosed;
				wrapper.GrantReward += OnGrantReward;
				wrapper.BannerDisplayed += OnBannerDisplayed;
				wrapper.BannerRemoved += OnBannerRemoved;
			}
		}

		protected override void RemoveEventListeners() 
		{
			base.RemoveEventListeners();

			EventManager.RemoveListener(IDs.Events.DisableAds, OnDisableAds);

			foreach (IMediationAdsWrapperInterface wrapper in Wrappers)
			{
				wrapper.InterstitialClosed -= OnInterstitialClosed;
				wrapper.GrantReward -= OnGrantReward;
				wrapper.BannerDisplayed -= OnBannerDisplayed;
				wrapper.BannerRemoved -= OnBannerRemoved;
			}
		}
		#endregion

		#region Event Handlers
		private void OnDisableAds()
		{
			DisableAds();
		}

		private void OnInterstitialClosed()
		{
			EventManager.SendEvent(IDs.Events.EnableMusicAfterAd);
			if (InterstitialClosed != null)
				InterstitialClosed();
		}

		private void OnGrantReward()
		{
			EventManager.SendEvent(IDs.Events.EnableMusicAfterAd);
			if (GrantReward != null)
				GrantReward();
		}

		private void OnBannerDisplayed()
		{
			if (BannerDisplayed != null)
				BannerDisplayed();
		}

		private void OnBannerRemoved()
		{
			if (BannerRemoved != null)
				BannerRemoved();
		}
		#endregion

		#region Getters
		protected IMediationAdsWrapperInterface Wrapper
		{
			get { return (IMediationAdsWrapperInterface)CurrentWrapperBase; }
		}
		#endregion

		#region Helpers
		private bool Enabled
		{
			get 
			{
				return APIManager.Instance.State.AdsEnabled && APIManager.Instance.Settings.AdsEnabled;
			}
		}
		#endregion
	}
}
