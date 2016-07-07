using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;
using Zedarus.ToolKit;
#if API_PROMO_BATCH
using Batch;
#endif

namespace Zedarus.ToolKit.API
{
	public class BatchWrapperSettings : APIWrapperSettings
	{
		private string _apiKey = "";

		// TODO: this is madness. We need completely change the way parameters are passed into
		// API controllers and use dictioneries instead with int or enum keys
		public BatchWrapperSettings(object[] settings) : base(settings)
		{
			Assert.IsTrue(settings.Length > 0, "Incorrect number of parameters for Batch wrapper");
			Assert.IsTrue(settings[0].GetType() == typeof(string), "First parameter must be string");

			_apiKey = settings[0].ToString();
		}

		public string APIKey
		{
			get { return _apiKey; }
		}
	}

	public class BatchWrapper : APIWrapper<BatchWrapper>, IPromoWrapperInterface 
	{
		#region Parameters
		#endregion

		#region Events
		#endregion

		#region Properties
		#if API_PROMO_BATCH
		private BatchPlugin _plugin = null;
		#endif
		#endregion

		#region Setup
		protected override void Setup(APIWrapperSettings settings) 
		{
			#if API_PROMO_BATCH
			BatchWrapperSettings batchSettings = settings as BatchWrapperSettings;
			if (batchSettings != null)
			{
				Debug.Log("HELLO 2");
				GameObject batchGO = new GameObject("Batch");
				GameObject.DontDestroyOnLoad(batchGO);

				_plugin = batchGO.AddComponent<BatchPlugin>();

				Config config = new Config();
				#if UNITY_IPHONE
				config.IOSAPIKey = batchSettings.APIKey;
				#elif UNITY_ANDROID
				config.AndroidAPIKey = batchSettings.APIKey;
				#endif

				_plugin.StartPlugin(config);
			}

			#endif


			// UNITY LOCAL NOTIFICATIONS
			// TEMP
//
//			// TODO: this should be called several times on each launch to remove all pending notifications on app start
//			if (UnityEngine.iOS.NotificationServices.localNotificationCount > 0) {
//				Debug.Log(UnityEngine.iOS.NotificationServices.localNotifications[0].alertBody);
//				UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
//			}
//
//			var notif = new UnityEngine.iOS.LocalNotification();
//			notif.fireDate = System.DateTime.Now.AddSeconds(120);
//			notif.alertAction = "Play";
//			notif.alertBody = "Hello!";
//			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
//
//			// TODO: Warning! this will display iOS standart request for notifications popup
//			UnityEngine.iOS.NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
//
			// TEMP

			SendInitializedEvent();	// TODO: temp
		}

		protected override APIWrapperSettings ParseSettings(object[] settings)
		{
			return new BatchWrapperSettings(settings);
		}
		#endregion

		#region Controls

		#endregion

		#region Event Listeners
		protected override void CreateEventListeners() 
		{
			
		}

		protected override void RemoveEventListeners() 
		{
			
		}
		#endregion

		#region Event Handlers

		#endregion
	}
}
