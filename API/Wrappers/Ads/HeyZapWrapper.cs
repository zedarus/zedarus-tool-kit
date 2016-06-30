using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
#if API_ADS_HEYZAP
using Heyzap;
#endif

namespace Zedarus.ToolKit.API
{
	public class HeyZapWrapperSettings : APIWrapperSettings
	{
		private string _apiKey = "";

		public HeyZapWrapperSettings(object[] settings) : base(settings)
		{
			Assert.IsTrue(settings.Length > 0, "Incorrect number of parameters for HeyZap wrapper");
			Assert.IsTrue(settings[0].GetType() == typeof(string), "First parameter must be string");

			_apiKey = settings[0].ToString();
		}

		public string APIKey
		{
			get { return _apiKey; }
		}
	}

	public class HeyZapWrapper : APIWrapper<HeyZapWrapper>, IAdsWrapperInterface
	{
		#region Events
		public event Action InterstitialClosed;
		public event Action GrantReward;
		public event Action BannerDisplayed;
		public event Action BannerRemoved;
		#endregion

		#region Properties
		private bool _bannerDisplayed = false;
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings)
		{
			#if API_ADS_HEYZAP
			HeyZapWrapperSettings heyzapSettings = settings as HeyZapWrapperSettings;
			if (heyzapSettings != null)
			{
				HeyzapAds.Start(heyzapSettings.APIKey, HeyzapAds.FLAG_NO_OPTIONS);
			}
			#endif
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return new HeyZapWrapperSettings(settings);
		}
		#endregion

		#region Controls - Caching
		public void CacheInterstitial(string tag)
		{
			#if API_ADS_HEYZAP
//			Debug.Log("CacheInterstitial: " + tag);
			HZInterstitialAd.Fetch(tag);
			#endif
		}

		public void CacheRewardedVideo(string tag)
		{
			#if API_ADS_HEYZAP
			//Debug.Log("CacheRewardedVideo: " + tag);
			HZIncentivizedAd.Fetch(tag);
			#endif
		}
		#endregion

		#region Controls
		public void ShowBanner(string tag)
		{
			if (_bannerDisplayed)
				HideBanner();
			
			//Debug.Log("ShowBanner: " + tag);
			if (BannerDisplayed != null)
				BannerDisplayed();

			#if API_ADS_HEYZAP
			HZBannerShowOptions options = new HZBannerShowOptions();
			options.Tag = tag;
			options.Position = HZBannerShowOptions.POSITION_TOP;

			HZBannerAd.ShowWithOptions(options);
			#endif

			_bannerDisplayed = true;
		}

		public void HideBanner()
		{
			#if API_ADS_HEYZAP
			HZBannerAd.Hide();
			HZBannerAd.Destroy();
			#endif

			if (BannerRemoved != null)
				BannerRemoved();
			_bannerDisplayed = false;
		}

		public void ShowIntersitital(string tag)
		{
			#if API_ADS_HEYZAP
			HZShowOptions options = new HZShowOptions();
			options.Tag = tag;

			HZInterstitialAd.ShowWithOptions(options);
			HZInterstitialAd.Fetch(tag);	// TODO: do we really need this?
			#endif
		}

		public void ShowRewardedVideo(string tag)
		{
			#if API_ADS_HEYZAP
			HZIncentivizedShowOptions options = new HZIncentivizedShowOptions();
			options.Tag = tag;

			HZIncentivizedAd.ShowWithOptions(options);
			HZIncentivizedAd.Fetch(tag);
			#endif
		}

		public void ShowTestUI()
		{
			#if API_ADS_HEYZAP
			HeyzapAds.ShowMediationTestSuite();
			#endif
		}
		#endregion

		#region Queries
		public float GetBannerHeight()
		{
			return 33f * 2f + 8f;
		}

		public bool IsBannerVisible()
		{
			return _bannerDisplayed;
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			#if API_ADS_HEYZAP
			HZBannerAd.SetDisplayListener(OnBannerStateUpdate);
			HZInterstitialAd.SetDisplayListener(OnInterstitialStateUpdate);
			HZVideoAd.SetDisplayListener(OnInterstitialStateUpdate);
			HZIncentivizedAd.SetDisplayListener(OnIncentivizedStateUpdate);
			#endif
		}

		protected override void RemoveEventListeners()
		{
			#if API_ADS_HEYZAP
			HZBannerAd.SetDisplayListener(null);
			HZInterstitialAd.SetDisplayListener(null);
			HZVideoAd.SetDisplayListener(null);
			HZIncentivizedAd.SetDisplayListener(null);
			#endif
		}
		#endregion

		#region Event Handlers
		private void OnBannerStateUpdate(string state, string tag)
		{
			//Debug.Log("OnBannerStateUpdate: " + state + ", tag: " + tag);
			if (state == "loaded") {
				// Do something when the banner ad is loaded
			}
			if (state == "error") {
				_bannerDisplayed = false;
				if (BannerRemoved != null)
					BannerRemoved();
				// Do something when the banner ad fails to load (they can fail when refreshing after successfully loading)
			}
			if (state == "click") {
				// Do something when the banner ad is clicked, like pause your game
			}
		}

		private void OnInterstitialStateUpdate(string state, string tag)
		{
			if ( state.Equals("show") ) {
				// Do something when the ad shows, like pause your game
			}
			if ( state.Equals("hide") ) {
				// Do something after the ad hides itself
				if (InterstitialClosed != null)
					InterstitialClosed();
			}
			if ( state.Equals("click") ) {
				// Do something when an ad is clicked on
			}
			if ( state.Equals("failed") ) {
				// Do something when an ad fails to show
				if (InterstitialClosed != null)
					InterstitialClosed();
			}
			if ( state.Equals("available") ) {
				// Do something when an ad has successfully been fetched
			}
			if ( state.Equals("fetch_failed") ) {
				// Do something when an ad did not fetch
			}
			if ( state.Equals("audio_starting") ) {
				// The ad being shown will use audio. Mute any background music
			}
			if ( state.Equals("audio_finished") ) {
				// The ad being shown has finished using audio.
				// You can resume any background music.
			}
		}

		private void OnIncentivizedStateUpdate(string state, string tag)
		{
			if (state.Equals ("show")) {
				// Do something when the ad shows, like pause your game
			}
			if (state.Equals ("hide")) {
				// Do something after the ad hides itself
				if (InterstitialClosed != null)
					InterstitialClosed();
			}
			if (state.Equals ("click")) {
				// Do something when an ad is clicked on
			}
			if (state.Equals ("failed")) {
				// Do something when an ad fails to show
				if (InterstitialClosed != null)
					InterstitialClosed();
			}
			if (state.Equals ("available")) {
				// Do something when an ad has successfully been fetched
			}
			if (state.Equals ("fetch_failed")) {
				// Do something when an ad did not fetch
			}
			if (state.Equals ("incentivized_result_complete")) {
				// The user has watched the entire video and should be given a reward.
				if (GrantReward != null)
					GrantReward();
			}
			if (state.Equals ("incentivized_result_incomplete")) {
				// The user did not watch the entire video and should not be given a reward.
			}
		}
		#endregion
	}
}
