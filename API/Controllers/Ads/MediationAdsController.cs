using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

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

		#region Settings
		private const string defaultTag = "default";
		private const string bootup = "bootup";
		private const string crosspromo = "cross_promo";
		private const string postLevelTag = "post_level";
		private const int postLevelMax = 2;
		#endregion

		#region Properties
		private bool _cached = false;
		#endregion

		#region Initialization
		protected override void Setup() {}	
		#endregion

		#region Wrappers Initialization
		protected override IAPIWrapperInterface GetWrapperForAPI(APIs wrapperAPI)
		{
			switch (wrapperAPI)
			{
				case APIs.HeyZap:
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
			
			CacheInterstitial(bootup);

			//CacheRewardedVideo("coins_shop");

			for (int i = 0; i < postLevelMax; i++)
			{
				CacheInterstitial(GetPostLevelTag(i));
				CacheVideo(GetPostLevelTag(i));
			}

			_cached = true;
		}

		public void CacheInterstitial(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CacheInterstitial(tag);
		}

		public void CacheVideo(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CacheVideo(tag);
		}

		public void CacheRewardedVideo(string tag)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.CacheRewardedVideo(tag);
		}
		#endregion

		#region Helpers
		public void ShowBootup()
		{
			ShowIntersitital(bootup, false);
		}

		public void ShowCrossPromo()
		{
			ShowIntersitital(crosspromo, false);
		}

		public void ShowBetweenLevelAd()
		{
			// TODO; check interstitial counter here:
			/*ShowIntersitital(GetPostLevelTag(PlayerDataManager.Instance.InterstitialsCounter), true);
			PlayerDataManager.Instance.InterstitialsCounter++;
			if (PlayerDataManager.Instance.InterstitialsCounter >= postLevelMax)
				PlayerDataManager.Instance.InterstitialsCounter = 0;*/
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
						//PlayerDataManager.Instance.ResetIntersititalsEvents();
						//AudioManager.Instance.SetMusicVolume(0f);
						#if UNITY_EDITOR
						OnInterstitialClosed();
						#else
						wrapper.ShowIntersitital(tag);
						#endif
					}
				}
				else
				{
					//AudioManager.Instance.SetMusicVolume(0f);
					#if UNITY_EDITOR
					OnInterstitialClosed();
					#else
					wrapper.ShowIntersitital(tag);
					#endif
				}
			}
		}

		public void ShowVideo(string tag, bool useBetweenLevelCounter)
		{
			IMediationAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
			{
				if (useBetweenLevelCounter)
				{
					if (ShouldDisplayBetweenLevelAd())
					{
						Debug.Log("Display video: " + tag);
						//PlayerDataManager.Instance.ResetIntersititalsEvents();
						wrapper.ShowVideo(tag);
					}
				}
				else
					wrapper.ShowVideo(tag);
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
						// TODO: AudioManager.Instance.SetMusicVolume(0f);
						// TODO: PlayerDataManager.Instance.ResetIntersititalsEvents();
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
					// TODO: AudioManager.Instance.SetMusicVolume(0f);
					#if UNITY_EDITOR
					OnInterstitialClosed();
					#else
					wrapper.ShowRewardedVideo(tag);
					#endif
				}
			}
		}

		public void DisableAds()
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

		public bool ShouldDisplayBetweenLevelAd()
		{
			if (Enabled)
				return true;
				// TODO: return PlayerDataManager.Instance.GetInterstitialsEvents() >= GlobalSettings.Instance.InterstitialsLevelsInterval;
			else
				return false;
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

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
		private void OnInterstitialClosed()
		{
			// TODO: AudioManager.Instance.SetMusicVolume(1f);
			if (InterstitialClosed != null)
				InterstitialClosed();
		}

		private void OnGrantReward()
		{
			// TODO: AudioManager.Instance.SetMusicVolume(1f);
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
				return true;
				// TODO: return GlobalSettings.Instance.AdsEnabled && PlayerDataManager.Instance.AdsEnabled;
			}
		}

		private string GetPostLevelTag(int counter)
		{
			return postLevelTag + "_" + (counter + 1).ToString();
		}
		#endregion
	}
}
