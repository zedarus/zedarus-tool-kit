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
					wrapper.CacheInterstitial(tag);
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
					wrapper.CacheRewardedVideo(tag);
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

		public void ShowBetweenLevelAd(string tag, Action callback)
		{
			ShowIntersitital(tag, callback, true);
		}

		public void ShowIntersitital(string tag, Action callback)
		{
			ShowIntersitital(tag, callback, false);
		}

		private void ShowIntersitital(string tag, Action callback, bool useBetweenLevelCounter)
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

				if (allowed)
				{
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
				DelayedCall.Create(OnInterstitialClosed, 2f);
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
					return Manager.State.IntertitialCounter >= Manager.Settings.IntertitialsDelay;
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
				return Manager.State.AdsEnabled && Manager.Settings.AdsEnabled;
			}
		}
		#endregion
	}
}
