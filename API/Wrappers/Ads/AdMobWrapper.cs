using UnityEngine;
using System.Collections;
using Zedarus.ToolKit;
using Zedarus.Traffico.Settings;

namespace Zedarus.ToolKit.API
{
	public class AdMobWrapper : APIWrapper<AdMobWrapper>, IBannerAdsWrapperInterface
	{
		#region Setup
		protected override void Setup()
		{
			#if UNITY_ANDROID
			//AdMobAndroid.init(publisherID);
			AdMobAndroid.setTestDevices(new string[] {"41078d212599bfd1", "4df15fd82135af03", "015d3248b71ffa0d"});
			#endif
		}
		#endregion

		#region Controls
		public void CreateBanner()
		{
			#if UNITY_ANDROID
			AdMobAndroid.createBanner(APIManager.Instance.Settings.AdMobUnitID, AdMobAndroidAd.phone320x50, AdMobAdPlacement.TopCenter);
			AdMobAndroid.hideBanner(true);
			#endif
		}

		public void ShowBanner()
		{
			#if UNITY_ANDROID
			AdMobAndroid.hideBanner(false);
			#endif
		}

		public void HideBanner()
		{
			#if UNITY_ANDROID
			AdMobAndroid.hideBanner(true);
			#endif
		}
		#endregion

		#region Queries
		public float GetBannerHeight()
		{
			#if UNITY_ANDROID
			return AdMobAndroid.getAdViewHeight();
			#else
			return 0f;
			#endif
		}
		#endregion

		#region Event Listeners
		protected override void CreateEventListeners()
		{
			#if UNITY_ANDROID
			AdMobAndroidManager.dismissingScreenEvent += dismissingScreenEvent;
			AdMobAndroidManager.failedToReceiveAdEvent += failedToReceiveAdEvent;
			AdMobAndroidManager.leavingApplicationEvent += leavingApplicationEvent;
			AdMobAndroidManager.presentingScreenEvent += presentingScreenEvent;
			AdMobAndroidManager.receivedAdEvent += receivedAdEvent;
			AdMobAndroidManager.interstitialDismissingScreenEvent += interstitialDismissingScreenEvent;
			AdMobAndroidManager.interstitialFailedToReceiveAdEvent += interstitialFailedToReceiveAdEvent;
			AdMobAndroidManager.interstitialLeavingApplicationEvent += interstitialLeavingApplicationEvent;
			AdMobAndroidManager.interstitialPresentingScreenEvent += interstitialPresentingScreenEvent;
			AdMobAndroidManager.interstitialReceivedAdEvent += interstitialReceivedAdEvent;
			#endif
		}
		
		protected override void RemoveEventListeners()
		{
			#if UNITY_ANDROID
			AdMobAndroidManager.dismissingScreenEvent -= dismissingScreenEvent;
			AdMobAndroidManager.failedToReceiveAdEvent -= failedToReceiveAdEvent;
			AdMobAndroidManager.leavingApplicationEvent -= leavingApplicationEvent;
			AdMobAndroidManager.presentingScreenEvent -= presentingScreenEvent;
			AdMobAndroidManager.receivedAdEvent -= receivedAdEvent;
			AdMobAndroidManager.interstitialDismissingScreenEvent -= interstitialDismissingScreenEvent;
			AdMobAndroidManager.interstitialFailedToReceiveAdEvent -= interstitialFailedToReceiveAdEvent;
			AdMobAndroidManager.interstitialLeavingApplicationEvent -= interstitialLeavingApplicationEvent;
			AdMobAndroidManager.interstitialPresentingScreenEvent -= interstitialPresentingScreenEvent;
			AdMobAndroidManager.interstitialReceivedAdEvent -= interstitialReceivedAdEvent;
			#endif
		}
		#endregion

		#region Event Handlers
		private void dismissingScreenEvent()
		{
			ZedLogger.Log("dismissingScreenEvent");
		}
		
		private void failedToReceiveAdEvent(string error)
		{
			ZedLogger.Log("failedToReceiveAdEvent: " + error);
		}
		
		private void leavingApplicationEvent()
		{
			ZedLogger.Log("leavingApplicationEvent");
		}
		
		private void presentingScreenEvent()
		{
			ZedLogger.Log("presentingScreenEvent");
		}
		
		private void receivedAdEvent()
		{
			ZedLogger.Log("receivedAdEvent");
		}
		
		private void interstitialDismissingScreenEvent()
		{
			ZedLogger.Log("interstitialDismissingScreenEvent");
		}
		
		private void interstitialFailedToReceiveAdEvent(string error)
		{
			ZedLogger.Log("interstitialFailedToReceiveAdEvent: " + error);
		}
		
		private void interstitialLeavingApplicationEvent()
		{
			ZedLogger.Log("interstitialLeavingApplicationEvent");
		}
		
		private void interstitialPresentingScreenEvent()
		{
			ZedLogger.Log("interstitialPresentingScreenEvent");
		}
		
		private void interstitialReceivedAdEvent()
		{
			ZedLogger.Log("interstitialReceivedAdEvent");
		}
		#endregion
	}
}
