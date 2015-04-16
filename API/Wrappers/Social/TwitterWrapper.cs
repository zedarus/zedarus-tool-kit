using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Settings;
using Zedarus.ToolKit;
#if API_TWITTER_P31
using Prime31;
#endif

namespace Zedarus.ToolKit.API
{
	#if API_TWITTER_P31
	public class TwitterWrapper : APIWrapper<TwitterWrapper>, ISocialWrapperInterface 
	{
		#region Events
		public event Action SharingStarted;
		public event Action<bool> SharingFinished;
		#endregion
		
		#region Setup
		protected override void Setup() 
		{
			TwitterCombo.init(APIManager.Instance.Settings.TwitterKey, APIManager.Instance.Settings.TwitterSecret);
		}
		#endregion
		
		#region Controls
		public void PostTextAndImage(string subject, string text, string imagePath, byte[] image, string url)
		{
			SendSharingStartedEvent();

			if (CanTweet)
				TwitterCombo.postStatusUpdate(text + ((url == null) ? "" : (" (" + url + ")")) + " #Numerity", imagePath);
			else
			{
				TwitterCombo.showLoginDialog();
				SendSharingFailedEvent();
			}
		}
		
		public void Share(string link, string name, string caption, string description, string pictureURL) 
		{
			SendSharingStartedEvent();

			if (CanTweet)
			{
				#if UNITY_IPHONE
				TwitterBinding.showTweetComposer(description + " #Numerity", pictureURL, link);
				#elif UNITY_ANDROID
				TwitterAndroid.postStatusUpdate(description + ((link == null) ? "" : (" (" + link + ")")) + " #Numerity");
				#endif
			}
			else
			{
				TwitterCombo.showLoginDialog();
				SendSharingFailedEvent();
			}
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			TwitterManager.loginFailedEvent += OnTwitterLoginFailedEvent;
			TwitterManager.loginSucceededEvent += OnTwitterLoginSucceededEvent;
			TwitterManager.requestDidFailEvent += OnTwitterRequestDidFailEvent;
			TwitterManager.requestDidFinishEvent += OnTwitterRequestDidFinishEvent;
			TwitterManager.tweetSheetCompletedEvent += OnTwitterTweetSheetCompletedEvent;
		}
		
		protected override void RemoveEventListeners() 
		{
			TwitterManager.loginFailedEvent -= OnTwitterLoginFailedEvent;
			TwitterManager.loginSucceededEvent -= OnTwitterLoginSucceededEvent;
			TwitterManager.requestDidFailEvent -= OnTwitterRequestDidFailEvent;
			TwitterManager.requestDidFinishEvent -= OnTwitterRequestDidFinishEvent;
			TwitterManager.tweetSheetCompletedEvent -= OnTwitterTweetSheetCompletedEvent;
		}
		#endregion
	
		#region Event Handlers
		private void OnTwitterLoginFailedEvent(string message) 
		{
			ZedLogger.Log("OnTwitterLoginFailedEvent: " + message);
		}
		
		private void OnTwitterLoginSucceededEvent(string message) 
		{
			ZedLogger.Log("OnTwitterLoginSucceededEvent: " + message);
			SendInitializedEvent();
		}
		
		private void OnTwitterRequestDidFailEvent(string message) 
		{
			ZedLogger.Log("OnTwitterRequestDidFailEvent: " + message);
			SendSharingFailedEvent();
		}

		private void OnTwitterRequestDidFinishEvent(object result) 
		{
			ZedLogger.Log("OnTwitterRequestDidFinishEvent: " + result);
			if (result != null)
				SendSharingSuccessEvent();
			else
				SendSharingFailedEvent();
		}
		
		private void OnTwitterTweetSheetCompletedEvent(bool result) 
		{
			ZedLogger.Log("OnTwitterTweetSheetCompletedEvent: " + result);
			if (result)
				SendSharingSuccessEvent();
			else
				SendSharingFailedEvent();
		}
		#endregion
		
		#region Event Senders
		private void SendSharingSuccessEvent()
		{
			if (SharingFinished != null)
				SharingFinished(true);
		}
		
		private void SendSharingFailedEvent()
		{
			if (SharingFinished != null)
				SharingFinished(false);
		}
		
		private void SendSharingStartedEvent()
		{
			if (SharingStarted != null)
				SharingStarted();
		}
		#endregion

		#region Helpers
		private bool CanTweet
		{
			get
			{
				#if UNITY_IPHONE
				return TwitterBinding.canUserTweet();
				#else
				return TwitterCombo.isLoggedIn();
				#endif
			}
		}
		#endregion
	}
	#endif
}
