using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.Traffico.Settings;
using Zedarus.ToolKit;
#if API_EMAIL_P31
using Prime31;
#endif

namespace Zedarus.ToolKit.API
{
#if API_EMAIL_P31
	public class EmailWrapper : APIWrapper<EmailWrapper>, ISocialWrapperInterface 
	{
		#region Events
		public event Action SharingStarted;
		public event Action<bool> SharingFinished;
		#endregion
		
		#region Setup
		protected override void Setup() 
		{
			#if UNITY_IPHONE
			if (EtceteraBinding.isEmailAvailable())
				SendInitializedEvent();
			#elif UNITY_ANDROID
			SendInitializedEvent();
			#endif
		}
		#endregion
		
		#region Controls
		public void PostTextAndImage(string subject, string text, string imagePath, byte[] image, string url)
		{
			#if UNITY_IPHONE
			SendSharingStartedEvent();
			if (EtceteraBinding.isEmailAvailable())
			{
				EtceteraManager.mailComposerFinishedEvent += OnMailComposerFinishedEvent;
				EtceteraBinding.showMailComposerWithAttachment(imagePath, "image/png", "kubiko_screenshot.png", "", subject, text + ((url == null) ? "" : (" (" + url + ")")), true);
			}
			else
				SendSharingFailedEvent();
			#elif UNITY_ANDROID
			EtceteraAndroid.showEmailComposer("", subject, text + ((url == null) ? "" : (" (" + url + ")")), true, imagePath);
			#else
			SendSharingFailedEvent();
			#endif
		}
		
		public void Share(string link, string name, string caption, string description, string pictureURL) 
		{
			#if UNITY_IPHONE
			SendSharingStartedEvent();

			if (EtceteraBinding.isEmailAvailable())
			{
				EtceteraManager.mailComposerFinishedEvent += OnMailComposerFinishedEvent;
				EtceteraBinding.showMailComposer("", name, string.Format(description, link, pictureURL), true);
			}
			else
				SendSharingFailedEvent();
			#elif UNITY_ANDROID
			EtceteraAndroid.showEmailComposer("", name, string.Format(description, link, pictureURL), true);
			#else
			SendSharingFailedEvent();
			#endif
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() {}
		protected override void RemoveEventListeners() {}
		#endregion
	
		#region Event Handlers
		private void OnMailComposerFinishedEvent(string message) 
		{
			#if UNITY_IPHONE
			EtceteraManager.mailComposerFinishedEvent -= OnMailComposerFinishedEvent;
			#endif
			ZedLogger.Log("Email composer finished with message: " + message);
			
			if (message.Equals("Sent"))
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
		
	}
#endif
}
