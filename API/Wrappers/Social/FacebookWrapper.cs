using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;

namespace Zedarus.ToolKit.API
{
	/*public class FacebookWrapper : APIWrapper<FacebookWrapper>, ISocialWrapperInterface 
	{
		#region Events
		public event Action SharingStarted;
		public event Action<bool> SharingFinished;
		#endregion
		
		#region Setup
		protected override void Setup() 
		{
			//ZedLogger.Log("init facebook");
			FacebookCombo.init();
		}
		#endregion
		
		#region Controls
		private void Login()
		{
			FacebookCombo.login();
		}	
		
		public void PostTextAndImage(string subject, string text, string imagePath, byte[] image, string url)
		{
			if (LoggedIn)
			{
				SendSharingStartedEvent();

				ZedLogger.Log("Facebook session is valid");
				#if UNITY_IPHONE
				if (FacebookBinding.canUserUseFacebookComposer())
				{
					ZedLogger.Log("Using facebook composer");
					FacebookBinding.showFacebookComposer(text + ((url == null) ? "" : (" (" + url + ")")), imagePath, null);
				}
				else
				#endif
				{
					if (FacebookCombo.getSessionPermissions().Contains("publish_actions"))
					{
						ZedLogger.Log("Facebook composer is not available, using alternative sharing method.");
						Facebook.instance.postImage(image, text + ((url == null) ? "" : (" (" + url + ")")), FacebookPostCompletionHandler);
					}
					else
					{
						ZedLogger.Log("Facebook permissions are invalid, trying to relogin");
						SendSharingFailedEvent();
						Login();
						return;
					}
				}
			}
			else
			{
				ZedLogger.Log("Facebook session is invalid, trying to relogin");
				SendSharingFailedEvent();
			}
		}
		
		public void Share(string link, string name, string caption, string description, string pictureURL) 
		{
			if (LoggedIn)
			{
				SendSharingStartedEvent();

				ZedLogger.Log("Facebook session is valid");
				Dictionary<string, object> options = new Dictionary<string, object>();
				options.Add("link", link);
				options.Add("name", name);
				options.Add("caption", caption);
				options.Add("description", description);
				options.Add("picture", pictureURL);
				FacebookCombo.showFacebookShareDialog(options);
			}
			else
			{
				ZedLogger.Log("Facebook session is invalid, trying to relogin");
				SendSharingFailedEvent();
				Login();
			}
		}
		#endregion
		
		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			FacebookManager.sessionOpenedEvent += OnFacebookLoginSuccessEvent;
			FacebookManager.preLoginSucceededEvent += OnFacebookPreLoginSuccessEvent;
			FacebookManager.loginFailedEvent += OnFacebookLoginFailedEvent;
			FacebookManager.dialogCompletedWithUrlEvent += OnFacebookDialogCompletedWithUrlEvent;
			FacebookManager.dialogFailedEvent += OnFacebookDialogFailedEvent;
			FacebookManager.graphRequestCompletedEvent += OnFacebookGraphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent += OnFacebookGraphRequestFailedEvent;
			FacebookManager.facebookComposerCompletedEvent += OnFacebookComposerCompletedEvent;
			FacebookManager.reauthorizationSucceededEvent += OnFacebookReauthorizationSucceededEvent;
			FacebookManager.reauthorizationFailedEvent += OnFacebookReauthorizationFailedEvent;
			FacebookManager.shareDialogFailedEvent += OnFacebookShareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent += OnFacebookShareDialogSucceededEvent;
		}
		
		protected override void RemoveEventListeners() 
		{
			FacebookManager.sessionOpenedEvent -= OnFacebookLoginSuccessEvent;
			FacebookManager.preLoginSucceededEvent -= OnFacebookPreLoginSuccessEvent;
			FacebookManager.loginFailedEvent -= OnFacebookLoginFailedEvent;
			FacebookManager.dialogCompletedWithUrlEvent -= OnFacebookDialogCompletedWithUrlEvent;
			FacebookManager.dialogFailedEvent -= OnFacebookDialogFailedEvent;
			FacebookManager.graphRequestCompletedEvent -= OnFacebookGraphRequestCompletedEvent;
			FacebookManager.graphRequestFailedEvent -= OnFacebookGraphRequestFailedEvent;
			FacebookManager.facebookComposerCompletedEvent -= OnFacebookComposerCompletedEvent;
			FacebookManager.reauthorizationSucceededEvent -= OnFacebookReauthorizationSucceededEvent;
			FacebookManager.reauthorizationFailedEvent -= OnFacebookReauthorizationFailedEvent;
			FacebookManager.shareDialogFailedEvent -= OnFacebookShareDialogFailedEvent;
			FacebookManager.shareDialogSucceededEvent -= OnFacebookShareDialogSucceededEvent;
		}
		#endregion
	
		#region Event Handlers
		private void OnFacebookComposerCompletedEvent(bool result) 
		{
			if (result)
				SendSharingSuccessEvent();
			else
				SendSharingFailedEvent();
		}
		
		private void OnFacebookLoginFailedEvent(P31Error error) {}
		
		private void OnFacebookLoginSuccessEvent() 
		{
			if (GlobalSettings.Instance.DevelopmentBuild) Debug.Log("OnFacebookLoginSuccessEvent");
			SendInitializedEvent();

			if (!FacebookCombo.getSessionPermissions().Contains("publish_actions"))
			{
				var permissions = new string[] { "publish_actions", "publish_stream" };
				FacebookCombo.reauthorizeWithPublishPermissions(permissions, FacebookSessionDefaultAudience.Everyone);
			}
		}
		
		private void OnFacebookPreLoginSuccessEvent() {}
		private void OnFacebookDialogCompletedWithUrlEvent(string url) 
		{
			ZedLogger.Log("OnFacebookDialogCompletedWithUrlEvent");
			SendSharingSuccessEvent();
		}
		private void OnFacebookDialogFailedEvent(P31Error error) 
		{
			ZedLogger.Log("OnFacebookDialogFailedEvent");
			SendSharingFailedEvent();
		}
		private void OnFacebookGraphRequestCompletedEvent(object result) {}
		private void OnFacebookGraphRequestFailedEvent(P31Error error) {}
		private void OnFacebookReauthorizationSucceededEvent() {}
		private void OnFacebookReauthorizationFailedEvent(P31Error error) {}
		private void OnFacebookShareDialogFailedEvent(P31Error error) 
		{
			ZedLogger.Log("Facebook sharing failed");
			SendSharingFailedEvent();
		}
		private void OnFacebookShareDialogSucceededEvent(Dictionary<string,object> result) 
		{
			ZedLogger.Log("Facebook sharing succeeded");
			SendSharingSuccessEvent();
		}
		#endregion
		
		#region Callbacks
		private void FacebookPostCompletionHandler(string a, object e)
		{
			ZedLogger.Log("FacebookPostCompletionHandler: " + a + ", " + e);
			if (e != null)
			{
				Dictionary<string, object> response = e as Dictionary<string, object>;
				foreach (string key in response.Keys)
				{
					ZedLogger.Log(key + " : " + response[key]);
				}
			}
			
			SendSharingSuccessEvent();
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
		private bool LoggedIn
		{
			get { return FacebookCombo.isSessionValid(); }
		}
		#endregion
	}*/
}
