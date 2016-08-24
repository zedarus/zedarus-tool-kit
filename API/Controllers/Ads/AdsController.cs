using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
using Zedarus.ToolKit.Data;
using Zedarus.ToolKit.Settings;
using Zedarus.ToolKit.Events;

namespace Zedarus.ToolKit.API
{
	public class AdsController : APIController 
	{	
		#region Events
		public event Action InterstitialClosed;
		public event Action GrantReward;
		public event Action BannerDisplayed;
		public event Action BannerRemoved;
		#endregion

		#region Properties
		private bool _interstitialsCached = false;
		private bool _rewardVideosCached = false;
		private Action _interstitialClosedCallback = null;
		private Action<int> _rewardCallback = null;
		private int _rewardProductID = 0;
		private int _testUIClicks = 0;
		private float _testUILastClickTime = 0f;
		private GameObject _adBlockObject = null;
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
		public void CacheIntersitials(string tag, params string[] otherTags)
		{
			if (_interstitialsCached)
				return;

			IAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
			{
				wrapper.CacheInterstitial(tag);
				foreach (string otherTag in otherTags)
				{
					wrapper.CacheInterstitial(otherTag);
				}
			}

			_interstitialsCached = true;
		}

		public void CacheRewardVideos(string tag, params string[] otherTags)
		{
			if (_rewardVideosCached)
				return;

			IAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
			{
				wrapper.CacheRewardedVideo(tag);
				foreach (string otherTag in otherTags)
				{
					wrapper.CacheRewardedVideo(otherTag);
				}
			}

			_rewardVideosCached = true;
		}
		#endregion

		#region Controls
		public void ShowBanner(string tag)
		{
			IAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				wrapper.ShowBanner(tag);
		}

		public void HideBanner()
		{
			IAdsWrapperInterface wrapper = Wrapper;
			if (wrapper != null)
				wrapper.HideBanner();
		}

		public void ShowBetweenLevelAd(string tag, Action callback, GameObject adBlockObject = null)
		{
			ShowIntersitital(tag, callback, true, false, adBlockObject);
		}

		public void ShowIntersitital(string tag, Action callback, GameObject adBlockObject = null)
		{
			ShowIntersitital(tag, callback, false, false, adBlockObject);
		}

		public void ShowPromo(string tag, Action callback, GameObject adBlockObject = null)
		{
			ShowIntersitital(tag, callback, false, true, adBlockObject);
		}

		private void ShowIntersitital(string tag, Action callback, bool useBetweenLevelCounter, bool usePromoCounter, GameObject adBlockObject)
		{
			IAdsWrapperInterface wrapper = Wrapper;
			bool adStarted = false;

			if (Enabled && wrapper != null)
			{
				if (useBetweenLevelCounter)
				{
					Manager.State.IncreaseInterstitialCounter();
				}

				bool allowed = true;

				if (useBetweenLevelCounter)
				{
					allowed = CanDisplayBetweenLevelAd;

					if (allowed)
					{
						Manager.State.ResetInterstitialCounter();
					}
				}

				if (usePromoCounter)
				{
					allowed = CanDisplayPromo;

					if (allowed)
					{
						Manager.State.RegisterPromoDisplay();
					}
				}

				if (allowed)
				{
					if (adBlockObject != null)
					{
						_adBlockObject = adBlockObject;
						_adBlockObject.SetActive(true);
					}

					adStarted = true;
					_interstitialClosedCallback = callback;
					EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
					#if UNITY_EDITOR
					DelayedCall.Create(OnInterstitialClosed, 2f);
					#else
					wrapper.ShowIntersitital(tag);
					#endif
				}
			}

			if (!adStarted && callback != null)
			{
				callback();
			}

			callback = null;
		}

		public void ShowRewardedVideo(string tag, Action callback, Action<int> rewardCallback, int productID)
		{
			IAdsWrapperInterface wrapper = Wrapper;
			bool adStarted = false;

			if (wrapper != null)
			{
				adStarted = true;
				_interstitialClosedCallback = callback;
				_rewardCallback = rewardCallback;
				_rewardProductID = productID;
				EventManager.SendEvent(IDs.Events.DisableMusicDuringAd);
				#if UNITY_EDITOR
				DelayedCall.Create(OnGrantReward, 2f);
				#else
				wrapper.ShowRewardedVideo(tag);
				#endif
			}

			if (!adStarted && callback != null)
			{
				callback();
			}

			callback = null;
			rewardCallback = null;
		}

		public void ShowTestUI(bool useClickCounter)
		{
			if (useClickCounter)
			{
				if (Time.realtimeSinceStartup - _testUILastClickTime <= 0.5f)
				{
					_testUIClicks++;
				}
				else
				{
					_testUIClicks = 0;
				}

				_testUILastClickTime = Time.realtimeSinceStartup;

				if (_testUIClicks >= 5)
				{
					_testUIClicks = 0;
					ShowTestUI(false);
				}
			}
			else
			{
				IAdsWrapperInterface wrapper = Wrapper;
				if (wrapper != null)
				{
					wrapper.ShowTestUI();
				}
			}
		}

		public void DisableAds()
		{
			HideBanner();
			Manager.State.DisableAds();
			EventManager.SendEvent(IDs.Events.AdsDisabled);
		}
		#endregion

		#region Queries
		public float GetBannerHeight()
		{
			IAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				return wrapper.GetBannerHeight();
			else
				return 0f;
		}

		public bool IsBannerVisible()
		{
			IAdsWrapperInterface wrapper = Wrapper;
			if (Enabled && wrapper != null)
				return wrapper.IsBannerVisible();
			else
				return false;
		}

		private bool CanDisplayBetweenLevelAd
		{
			get
			{
				if (Enabled)
				{
					return Manager.State.IntertitialCounter >= Manager.Settings.IntertitialsDelay;
				}
				else
					return false;
			}
		}

		private bool CanDisplayPromo
		{
			get 
			{
				if (Enabled)
				{
					if (Manager.Settings.PromoAdsEnabled)
					{
						int minutes = Convert.ToInt32((DateTime.UtcNow - Manager.State.LastPromoDisplayDate).TotalMinutes);
						return minutes >= Manager.Settings.PromoAdsDelayMins;
					}
					else
					{
						return false;
					}
				}
				else
					return false;
			}
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			base.CreateEventListeners();

			foreach (IAdsWrapperInterface wrapper in Wrappers)
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

			foreach (IAdsWrapperInterface wrapper in Wrappers)
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
			EventManager.SendEvent(IDs.Events.EnableMusicAfterAd);

			if (_adBlockObject != null)
			{
				_adBlockObject.SetActive(false);
				_adBlockObject = null;
			}

			if (InterstitialClosed != null)
				InterstitialClosed();

			if (_interstitialClosedCallback != null)
			{
				_interstitialClosedCallback();
				_interstitialClosedCallback = null;
			}
		}

		private void OnGrantReward()
		{
			EventManager.SendEvent(IDs.Events.EnableMusicAfterAd);
			if (GrantReward != null)
				GrantReward();

			if (_rewardCallback != null)
			{
				_rewardCallback(_rewardProductID);
				_rewardProductID = 0;
				_rewardCallback = null;
			}
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
		protected IAdsWrapperInterface Wrapper
		{
			get { return (IAdsWrapperInterface)CurrentWrapperBase; }
		}
		#endregion

		#region Helpers
		public bool Enabled
		{
			get 
			{
				return Manager.State.AdsEnabled && Manager.Settings.AdsEnabled && !FreeAdsRemovalEnabled;
			}
		}

		public bool FreeAdsRemovalEnabled
		{
			get
			{
				return FreeAdsRemovalMinutes <= Manager.Settings.FreeAdsRemovalDurationHours * 60;
			}
		}

		public int FreeAdsRemovalMinutes
		{
			get 
			{ 
				return Convert.ToInt32((DateTime.UtcNow - Manager.State.LastFreeAdsRemovalDate).TotalMinutes);; 
			}
		}

		public int FreeAdsRemovalMinutesRemaining
		{
			get 
			{ 
				return Manager.Settings.FreeAdsRemovalDurationHours * 60 - FreeAdsRemovalMinutes; 
			}
		}
		#endregion
	}
}
